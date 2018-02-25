using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Services;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using Imgur.API.Models.Impl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OMDbSharp;
using OverwatchAPI;
using PokemonTcgSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class CommonModule
    {
        [Command("8ball")]
        [Description("Roll an 8-ball")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task EightBall(CommandContext CTX, string question)
        {
            await CTX.TriggerTypingAsync();
            if (string.IsNullOrWhiteSpace(question))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a question");
            else
            {
                Random RND = new Random();
                List<string> EightBallAnswers = CommonService.Get8BallList();
                await CTX.RespondAsync(EightBallAnswers[RND.Next(0, EightBallAnswers.Count)]);
            }
        }

        [Command("avatar")]
        [Aliases("av")]
        [Description("Get mentioned user's avatar")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchUserAvatar(CommandContext CTX, DiscordMember member)
        {
            await CTX.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"{member.DisplayName}'s avatar, click here for the link...")
                .WithImageUrl($"{member.GetAvatarUrl(ImageFormat.Jpeg)}")
                .WithUrl($"{member.GetAvatarUrl(ImageFormat.Jpeg)}")
                .WithColor(DiscordColor.Blue);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("catfact")]
        [Aliases("cat")]
        [Description("Get a random cat fact")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetCatFact(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync("https://catfact.ninja/fact");
            await CTX.RespondAsync($":cat: Meow! {JObject.Parse(response)["fact"].ToString()}");
        }

        [Command("color")]
        [Description("Get color values corresponding to the inputted RGB")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetColor(CommandContext CTX, params string[] colors)
        {
            try
            {
                await CTX.TriggerTypingAsync();
                var RGB = colors.Length == 4;
                var ARG = colors[1].Replace("#", "");
                var red = Convert.ToByte(RGB ? int.Parse(ARG) : Convert.ToInt32(ARG.Substring(0, 2), 16));
                var green = Convert.ToByte(RGB ? int.Parse(colors[2]) : Convert.ToInt32(ARG.Substring(2, 2), 16));
                var blue = Convert.ToByte(RGB ? int.Parse(colors[3]) : Convert.ToInt32(ARG.Substring(4, 2), 16));
                DiscordColor color = new DiscordColor(red, green, blue);
                var output = new DiscordEmbedBuilder()
                    .AddField("**HEX:**", $"{color.Value.ToString("X")}")
                    .AddField("**RGB:**", $"{colors[1]} {colors[2]} {colors[3]}")
                    .AddField("**Decimal:**", $"{color.Value}")
                    .WithColor(color);
                await CTX.RespondAsync("", embed: output.Build());
            }
            catch
            {
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Unable to retrieve color values, use **.color # [0-255] [0-255] [0-255]**");
            }
        }

        [Command("define")]
        [Aliases("def")]
        [Description("Get a Dictionary definition for a word or phrase")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchDefinition(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a word or a phrase...");
            else
            {
                await CTX.TriggerTypingAsync();
                DefinitionService.Data data = new DefinitionService.Data();
                data = await DefinitionService.GetDefinitionForTermAsync(query);
                var sense = data.Results.FirstOrDefault(x => x.Senses?[0].Definition != null)?.Senses[0];

                if (sense?.Definition == null)
                    await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Unknown definition");

                var definition = sense.Definition.ToString();
                if (!(sense.Definition is string))
                    definition = ((JArray)JToken.Parse(sense.Definition.ToString())).First.ToString();

                var output = new DiscordEmbedBuilder()
                    .WithTitle($"Define {query}")
                    .WithDescription(definition)
                    .WithFooter(sense.Gramatical_info?.type)
                    .WithColor(DiscordColor.Blurple);

                if (sense.Examples != null)
                    output.AddField("Example", sense.Examples.First().text);

                await CTX.RespondAsync(embed: output.Build());
            }
        }

        [Command("dictionary")]
        [Aliases("dic")]
        [Description("Get an Urban Dictionary entry for a word or phrase")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchDictionary(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a word or a phrase...");
            else
            {
                await CTX.TriggerTypingAsync();
                DictionaryService.Data data = new DictionaryService.Data();
                data = await DictionaryService.GetDictionaryForTermAsync(query);
                if (data.ResultType == "no_results")
                    await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} No results found!");
                else
                {
                    foreach (var value in data.List)
                    {
                        var output = new DiscordEmbedBuilder()
                            .WithTitle($"Urban Dictionary definition for \"{query}\" by {value.Author}")
                            .WithDescription(value.Definition.Length < 1000 ? value.Definition : value.Definition.Take(1000) + "...")
                            .WithUrl(value.Permalink)
                            .WithFooter("Type next for the next definition")
                            .WithColor(DiscordColor.Blurple);
                        if (!string.IsNullOrWhiteSpace(value.Example))
                            output.AddField("Example", value.Example);
                        await CTX.RespondAsync(embed: output.Build());

                        var interactivity = await CTX.Client.GetInteractivityModule().WaitForMessageAsync(m => m.Channel.Id == CTX.Channel.Id && m.Content.ToLower() == "next", TimeSpan.FromSeconds(15));
                        if (interactivity == null)
                            break;
                    }
                }
            }
        }

        [Command("imdb")]
        [Aliases("omdb")]
        [Description("Get a movie or TV show from OMDB")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchIMDB(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a movie or TV show to search for...");
            else
            {
                await CTX.TriggerTypingAsync();
                APITokenService service = new APITokenService();
                string Token = service.GetAPIToken("omdb");
                OMDbClient client = new OMDbClient(Token, true);
                var movie = await client.GetItemByTitle(query.Replace(" ", "+"));
                if (movie.Response == "False")
                    await CTX.RespondAsync("The movie you were searching for was not found");
                else
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(movie.Title.ToString())
                        .WithDescription(movie.Plot.Length < 500 ? movie.Plot : movie.Plot.Take(500) + "...")
                        .AddField("Genre", movie.Genre, true)
                        .AddField("Released", movie.Released, true)
                        .AddField("Runtime", movie.Runtime, true)
                        .AddField("Production", movie.Production, true)
                        .AddField("Country", movie.Country, true)
                        .AddField("BoxOffice", movie.BoxOffice, true)
                        .AddField("IMDB Rating", movie.imdbRating, true)
                        .AddField("Metacritic", movie.Metascore, true)
                        .AddField("RottenTomato", movie.tomatoRating, true)
                        .AddField("Director", movie.Director, true)
                        .AddField("Actors", movie.Actors, true)
                        .WithColor(DiscordColor.CornflowerBlue);
                    if (movie.Poster != "N/A") output.WithImageUrl(movie.Poster);
                    await CTX.RespondAsync(embed: output.Build());
                }
            }
        }

        [Command("imgur")]
        [Description("Get an imager from Imgur")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Find(CommandContext CTX, [RemainingText] string query = null)
        {
            await CTX.TriggerTypingAsync();
            Random RND = new Random();
            APITokenService service = new APITokenService();
            string Token = service.GetAPIToken("imgur");
            ImgurClient imgur = new ImgurClient(Token);
            var endpoint = new GalleryEndpoint(imgur);
            List<IGalleryItem> gallery;
            if (String.IsNullOrWhiteSpace(query))
                gallery = (await endpoint.GetRandomGalleryAsync()).ToList();
            else
                gallery = (await endpoint.SearchGalleryAsync(query)).ToList();

            var IMG = gallery.Any() ? gallery[RND.Next(0, gallery.Count)] : null;
            if (IMG == null)
                await CTX.RespondAsync("Couldn't find anything");
            else
            {
                if (IMG is GalleryAlbum)
                    await CTX.RespondAsync(((GalleryAlbum)IMG).Link);
                else if (IMG is GalleryImage)
                    await CTX.RespondAsync(((GalleryImage)IMG).Link);
            }
        }

        [Command("math")]
        [Description("Perform a basic math operation")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Math(CommandContext CTX, double num1, MathOperations operation, double num2)
        {
            var result = 0.0;
            switch (operation)
            {
                case MathOperations.Add:
                    result = num1 + num2;
                    break;

                case MathOperations.Subtract:
                    result = num1 - num2;
                    break;

                case MathOperations.Multiply:
                    result = num1 * num2;
                    break;

                case MathOperations.Divide:
                    result = num1 / num2;
                    break;

                case MathOperations.Modulo:
                    result = num1 % num2;
                    break;
            }
            await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":1234:")} The result is {result.ToString("#,##0.00")}");
        }

        [Command("pokemon")]
        [Aliases("poke")]
        [Description("Get Pokemon information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task CheckPokemon(CommandContext CTX, string query)
        {
            await CTX.TriggerTypingAsync();
            PokemonService.RootObject pokemon = new PokemonService.RootObject();
            pokemon = await PokemonService.GetPokemonDataAsync(query);
            if (pokemon.cards.Count == 0)
                await CTX.RespondAsync($":warning: Unable to find this pokemon");
            else
            {
                Random RNG = new Random();
                var cards = Card.Find<Pokemon>(pokemon.cards[RNG.Next(0, pokemon.cards.Count)].id);
                var output = new DiscordEmbedBuilder()
                    .WithTitle($"{cards.Card.Name} (ID: {cards.Card.NationalPokedexNumber})")
                    .WithImageUrl(cards.Card.ImageUrl)
                    .WithColor(DiscordColor.Lilac)
                    .WithFooter($" Card ID: {cards.Card.Id}");
                await CTX.RespondAsync(embed: output.Build());
            }
        }

        [Command("overwatch")]
        [Aliases("ow")]
        [Description("Get Overwatch player information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchOverwatch(CommandContext CTX, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a battletag like CriticalFlaw#11354");
            else
            {
                await CTX.TriggerTypingAsync();
                using (var owClient = new OverwatchClient())
                {
                    Player player = await owClient.GetPlayerAsync(query);
                    if (player == null)
                        await CTX.RespondAsync($"The player you were searching for was not found (Battletags is text-sensitive)");
                    else
                    {
                        //var BarrierDamageDone = player.CasualStats.GetStatExact("All Heroes", "Combat", "Barrier Damage Done");
                        //var MeleeFinalBlows = player.CasualStats.GetStatExact("All Heroes", "Combat", "Melee Final Blows");
                        //var Deaths = player.CasualStats.GetStatExact("All Heroes", "Combat", "Deaths");
                        //var HeroDamageDone = player.CasualStats.GetStatExact("All Heroes", "Combat", "Hero Damage Done");
                        //var TimeSpentOnFire = player.CasualStats.GetStatExact("All Heroes", "Combat", "Time Spent On Fire");
                        //var SoloKills = player.CasualStats.GetStatExact("All Heroes", "Combat", "Solo Kills");
                        //var ObjectiveTime = player.CasualStats.GetStatExact("All Heroes", "Combat", "Objective Time");
                        //var ObjectiveKills = player.CasualStats.GetStatExact("All Heroes", "Combat", "Objective Kills");
                        //var FinalBlows = player.CasualStats.GetStatExact("All Heroes", "Combat", "Final Blows");
                        //var Eliminations = player.CasualStats.GetStatExact("All Heroes", "Combat", "Eliminations");
                        //var AllDamageDone = player.CasualStats.GetStatExact("All Heroes", "Combat", "All Damage Done");
                        //var EnvironmentalKills = player.CasualStats.GetStatExact("All Heroes", "Combat", "Environmental Kills");
                        //var MultiKills = player.CasualStats.GetStatExact("All Heroes", "Combat", "MultiKills");
                        //var Cards = player.CasualStats.GetStatExact("All Heroes", "Match Awards", "Cards");
                        //var Medals = player.CasualStats.GetStatExact("All Heroes", "Match Awards", "Medals");
                        //var MedalsGold = player.CasualStats.GetStatExact("All Heroes", "Match Awards", "Medals - Gold");
                        //var MedalsSilver = player.CasualStats.GetStatExact("All Heroes", "Match Awards", "Medals - Silver");
                        //var MedalsBronze = player.CasualStats.GetStatExact("All Heroes", "Match Awards", "Medals - Bronze");
                        //var TimePlayed = player.CasualStats.GetStatExact("All Heroes", "Game", "Time Played");
                        //var GamesWon = player.CasualStats.GetStatExact("All Heroes", "Game", "Games Won");
                        //var ShieldGeneratorDestroyed = player.CasualStats.GetStatExact("All Heroes", "Miscellaneous", "Shield Generator Destroyed");
                        //var TurretsDestroyed = player.CasualStats.GetStatExact("All Heroes", "Miscellaneous", "Turrets Destroyed");
                        //var HealingDone = player.CasualStats.GetStatExact("All Heroes", "Assists", "Healing Done");
                        //var ReconAssists = player.CasualStats.GetStatExact("All Heroes", "Assists", "Recon Assists");
                        //var TeleporterPadsDestroyed = player.CasualStats.GetStatExact("All Heroes", "Assists", "Teleporter Pads Destroyed");
                        //var OffensiveAssists = player.CasualStats.GetStatExact("All Heroes", "Assists", "Offensive Assists");
                        //var DefensiveAssists = player.CasualStats.GetStatExact("All Heroes", "Assists", "Defensive Assists");
                        //var AvgBarrierDamageDone = player.CasualStats.GetStatExact("All Heroes", "Average", "Barrier Damage Done - Avg Per 10 Min");
                        //var AvgDeaths = player.CasualStats.GetStatExact("All Heroes", "Average", "Deaths - Avg Per 10 Min");
                        //var AvgHeroDamageDone = player.CasualStats.GetStatExact("All Heroes", "Average", "Hero Damage Done - Avg Per 10 Min");
                        //var AvgTimeSpentOnFire = player.CasualStats.GetStatExact("All Heroes", "Average", "Time Spent On Fire - Avg Per 10 Min");
                        //var AvgSoloKills = player.CasualStats.GetStatExact("All Heroes", "Average", "Solo Kills - Avg Per 10 Min");
                        //var AvgObjectiveTime = player.CasualStats.GetStatExact("All Heroes", "Average", "Objective Time - Avg Per 10 Min");
                        //var AvgObjectiveKills = player.CasualStats.GetStatExact("All Heroes", "Average", "Objective Kills - Avg Per 10 Min");
                        //var AvgHealingDone = player.CasualStats.GetStatExact("All Heroes", "Average", "Healing Done - Avg Per 10 Min");
                        //var AvgFinalBlows = player.CasualStats.GetStatExact("All Heroes", "Average", "Final Blows - Avg Per 10 Min");
                        //var AvgEliminations = player.CasualStats.GetStatExact("All Heroes", "Average", "Eliminations - Avg Per 10 Min");
                        //var AvgAllDamageDone = player.CasualStats.GetStatExact("All Heroes", "Average", "All Damage Done - Avg Per 10 Min");
                        //var MostEliminations = player.CasualStats.GetStatExact("All Heroes", "Best", "Eliminations - Most In Game");
                        //var MostFinalBlows = player.CasualStats.GetStatExact("All Heroes", "Best", "Final Blows - Most In Game");
                        //var MostAllDamageDone = player.CasualStats.GetStatExact("All Heroes", "Best", "All Damage Done - Most In Game");
                        //var MostHealingDone = player.CasualStats.GetStatExact("All Heroes", "Best", "Healing Done - Most In Game");
                        //var MostDefensiveAssists = player.CasualStats.GetStatExact("All Heroes", "Best", "Defensive Assists - Most In Game");
                        //var MostOffensiveAssists = player.CasualStats.GetStatExact("All Heroes", "Best", "Offensive Assists - Most In Game");
                        //var MostObjectiveKills = player.CasualStats.GetStatExact("All Heroes", "Best", "Objective Kills - Most In Game");
                        //var MostObjectiveTime = player.CasualStats.GetStatExact("All Heroes", "Best", "Objective Time - Most In Game");
                        //var MostMultiKill = player.CasualStats.GetStatExact("All Heroes", "Best", "MultiKill - Best");
                        //var MostSoloKills = player.CasualStats.GetStatExact("All Heroes", "Best", "Solo Kills - Most In Game");
                        //var MostTimeSpentOnFire = player.CasualStats.GetStatExact("All Heroes", "Best", "Time Spent On Fire - Most In Game");
                        //var MostMeleeFinalBlows = player.CasualStats.GetStatExact("All Heroes", "Best", "Melee Final Blows - Most In Game");
                        //var MostShieldGeneratorDestroyed = player.CasualStats.GetStatExact("All Heroes", "Best", "Shield Generator Destroyed - Most In Game");
                        //var MostTurretsDestroyed = player.CasualStats.GetStatExact("All Heroes", "Best", "Turrets Destroyed - Most In Game");
                        //var MostEnviromentalKill = player.CasualStats.GetStatExact("All Heroes", "Best", "Enviromental Kill - Most In Game");
                        //var MostTeleporterPadDestroyed = player.CasualStats.GetStatExact("All Heroes", "Best", "Teleporter Pad Destroyed - Most In Game");
                        //var MostKillStreak = player.CasualStats.GetStatExact("All Heroes", "Best", "Kill Streak - Best");
                        //var MostHeroDamageDone = player.CasualStats.GetStatExact("All Heroes", "Best", "Hero Damage Done - Most In Game");
                        //var MostBarrierDamageDone = player.CasualStats.GetStatExact("All Heroes", "Best", "Barrier Damage Done - Most In Game");
                        //var MostReconAssists = player.CasualStats.GetStatExact("All Heroes", "Best", "Recon Assists - Most In Game");
                        var output = new DiscordEmbedBuilder()
                            .WithTitle(player.Username)
                            .AddField("Level", player.PlayerLevel.ToString(), true)
                            .AddField("Competitive", player.CompetitiveRank.ToString(), true)
                            .AddField("Platform", player.Platform.ToString().ToUpper(), true)
                            .AddField("Achievements", player.Achievements.Count().ToString(), true)
                            //.AddField("Barrier Damage Done", BarrierDamageDone.Value.ToString(), true)
                            //.AddField("Melee Final Blows", MeleeFinalBlows.Value.ToString(), true)
                            //.AddField("Deaths", Deaths.Value.ToString(), true)
                            //.AddField("Hero Damage Done", HeroDamageDone.Value.ToString(), true)
                            //.AddField("Time Spent On Fire", TimeSpentOnFire.Value.ToString(), true)
                            //.AddField("Solo Kills", SoloKills.Value.ToString(), true)
                            //.AddField("Objective Time", ObjectiveTime.Value.ToString(), true)
                            //.AddField("Objective Kills", ObjectiveKills.Value.ToString(), true)
                            //.AddField("Final Blows", FinalBlows.Value.ToString(), true)
                            //.AddField("Eliminations", Eliminations.Value.ToString(), true)
                            //.AddField("All Damage Done", AllDamageDone.Value.ToString(), true)
                            //.AddField("Environmental Kills", EnvironmentalKills.Value.ToString(), true)
                            //.AddField("MultiKills", MultiKills.Value.ToString(), true)
                            //.AddField("Cards", Cards.Value.ToString(), true)
                            //.AddField("Medals", Medals.Value.ToString(), true)
                            //.AddField("Medals Gold", MedalsGold.Value.ToString(), true)
                            //.AddField("Medals Silver", MedalsSilver.Value.ToString(), true)
                            //.AddField("Medals Bronze", MedalsBronze.Value.ToString(), true)
                            //.AddField("Time Played", TimePlayed.Value.ToString(), true)
                            //.AddField("Games Won", GamesWon.Value.ToString(), true)
                            //.AddField("Shield Generator Destroyed", ShieldGeneratorDestroyed.Value.ToString(), true)
                            //.AddField("Turrets Destroyed", TurretsDestroyed.Value.ToString(), true)
                            //.AddField("Healing Done", HealingDone.Value.ToString(), true)
                            //.AddField("Recon Assists", ReconAssists.Value.ToString(), true)
                            //.AddField("Teleporter Pads Destroyed", TeleporterPadsDestroyed.Value.ToString(), true)
                            //.AddField("Offensive Assists", OffensiveAssists.Value.ToString(), true)
                            //.AddField("Defensive Assists", DefensiveAssists.Value.ToString(), true)
                            //.AddField("Avg Barrier Damage Done", AvgBarrierDamageDone.Value.ToString(), true)
                            //.AddField("Avg Deaths", AvgDeaths.Value.ToString(), true)
                            //.AddField("Avg Hero Damage Done", AvgHeroDamageDone.Value.ToString(), true)
                            //.AddField("Avg TimeSpent On Fire", AvgTimeSpentOnFire.Value.ToString(), true)
                            //.AddField("Avg Solo Kills", AvgSoloKills.Value.ToString(), true)
                            //.AddField("Avg Objective Time", AvgObjectiveTime.Value.ToString(), true)
                            //.AddField("Avg Objective Kills", AvgObjectiveKills.Value.ToString(), true)
                            //.AddField("Avg Healing Done", AvgHealingDone.Value.ToString(), true)
                            //.AddField("Avg Final Blows", AvgFinalBlows.Value.ToString(), true)
                            //.AddField("Avg Eliminations", AvgEliminations.Value.ToString(), true)
                            //.AddField("Avg All Damage Done", AvgAllDamageDone.Value.ToString(), true)
                            //.AddField("Most Eliminations", MostEliminations.Value.ToString(), true)
                            //.AddField("Most Final Blows", MostFinalBlows.Value.ToString(), true)
                            //.AddField("Most AllDamage Done", MostAllDamageDone.Value.ToString(), true)
                            //.AddField("Most Healing Done", MostHealingDone.Value.ToString(), true)
                            //.AddField("Most Defensive Assists", MostDefensiveAssists.Value.ToString(), true)
                            //.AddField("Most Offensive Assists", MostOffensiveAssists.Value.ToString(), true)
                            //.AddField("Most Objective Kills", MostObjectiveKills.Value.ToString(), true)
                            //.AddField("Most Objective Time", MostObjectiveTime.Value.ToString(), true)
                            //.AddField("Most MultiKill", MostMultiKill.Value.ToString(), true)
                            //.AddField("Most SoloKills", MostSoloKills.Value.ToString(), true)
                            //.AddField("Most Time Spent On Fire", MostTimeSpentOnFire.Value.ToString(), true)
                            //.AddField("Most Melee Final Blows", MostMeleeFinalBlows.Value.ToString(), true)
                            //.AddField("Most Shield Generator Destroyed", MostShieldGeneratorDestroyed.Value.ToString(), true)
                            //.AddField("Most Turrets Destroyed", MostTurretsDestroyed.Value.ToString(), true)
                            //.AddField("Most Enviromental Kill", MostEnviromentalKill.Value.ToString(), true)
                            //.AddField("Most Teleporter Pad Destroyed", MostTeleporterPadDestroyed.Value.ToString(), true)
                            //.AddField("Most KillStreak", MostKillStreak.Value.ToString(), true)
                            //.AddField("Most Hero Damage Done", MostHeroDamageDone.Value.ToString(), true)
                            //.AddField("Most Barrier Damage Done", MostBarrierDamageDone.Value.ToString(), true)
                            //.AddField("Most Recon Assists", MostReconAssists.Value.ToString(), true)
                            .WithThumbnailUrl(player.ProfilePortraitUrl)
                            .WithUrl(player.ProfileUrl)
                            .WithColor(DiscordColor.Gold);
                        await CTX.RespondAsync(embed: output.Build());
                    }
                }
            }
        }

        [Command("randomcat")]
        [Aliases("meow")]
        [Description("Get a random cat image")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task RandomCat(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            using (WebClient client = new WebClient())
            {
                var image = client.DownloadString("http://random.cat/meow");
                int iFrom = image.IndexOf("\\/i\\/") + "\\/i\\/".Length;
                int iTo = image.LastIndexOf("\"}");
                string cat = image.Substring(iFrom, iTo - iFrom);
                await CTX.RespondAsync($":cat: Meow! http://random.cat/i/{cat}");
            }
        }

        [Command("randomdog")]
        [Aliases("woof")]
        [Description("Get a random dog image")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task RandomDog(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            using (WebClient client = new WebClient())
                await CTX.RespondAsync(":dog: Bork! http://random.dog/" + client.DownloadString("http://random.dog/woof"));
        }

        [Command("revav")]
        [Description("Reverse image search someone's avatar")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchAvatarReverse(CommandContext CTX, DiscordMember member)
        {
            await CTX.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("Google Reverse Image Search")
                .WithImageUrl(member.AvatarUrl)
                .WithUrl($"https://images.google.com/searchbyimage?image_url={member.AvatarUrl}")
                .WithColor(DiscordColor.DarkButNotBlack);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("simpsons")]
        [Aliases("doh")]
        [Description("Get a random Simpsons screenshot and episode")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SimpsonsEpisode(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            SimpsonsService.RootObject data = new SimpsonsService.RootObject();
            data = await SimpsonsService.GetSimpsonsDataAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle(data.Episode.Title)
                .AddField("Season/Episode", data.Episode.Key, true)
                .AddField("Air Date", data.Episode.OriginalAirDate, true)
                .AddField("Writer", data.Episode.Writer, true)
                .AddField("Director", data.Episode.Director, true)
                .WithImageUrl($"https://frinkiac.com/img/{data.Frame.Episode}/{data.Frame.Timestamp}.jpg")
                .WithUrl(data.Episode.WikiLink)
                .WithColor(DiscordColor.Yellow);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("simpsonsgif")]
        [Description("Get a random Simpsons gif")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SimpsonsGif(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            var data = await SimpsonsService.GetSimpsonsGifAsync();
            await CTX.RespondAsync($"**Note:** First time gifs take a few minutes to properly generate\n{data.ToString()}");
        }

        [Command("sum")]
        [Aliases("total")]
        [Description("Sum all inputted numbers")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SumOfNumbers(CommandContext CTX, params int[] args)
        {
            await CTX.TriggerTypingAsync();
            await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":1234:")} The sum is {args.Sum().ToString("#,##0")}");
        }

        [Command("time")]
        [Aliases("ti")]
        [Description("Get time for specified location")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetTime(CommandContext CTX, [RemainingText] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a location...");
            else
            {
                await CTX.TriggerTypingAsync();
                APITokenService service = new APITokenService();
                string Token = service.GetAPIToken("google");
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Clear();
                    var locationResource = await http.GetStringAsync($"https://maps.googleapis.com/maps/api/geocode/json?address={location.Replace(" ", "")}&key={Token}");
                    var locationObject = JsonConvert.DeserializeObject<TimeService>(locationResource);
                    var currentSeconds = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; //DateTime.UtcNow.ToUniversalTime();
                    var url = $"https://maps.googleapis.com/maps/api/timezone/json?location={locationObject.results[0].Geometry.Location.Lat},{locationObject.results[0].Geometry.Location.Lng}&timestamp={currentSeconds}&key={Token}";
                    var timeResource = await http.GetStringAsync(url);
                    var timeObject = JsonConvert.DeserializeObject<TimeService.TimeZoneResult>(timeResource);
                    var time = DateTime.UtcNow.AddSeconds(timeObject.DstOffset + timeObject.RawOffset);

                    var output = new DiscordEmbedBuilder()
                        .WithTitle($"Local Time in {location}")
                        .WithDescription($"{DiscordEmoji.FromName(CTX.Client, ":clock1:")}  **{time.ToShortTimeString()}**")
                        .WithColor(DiscordColor.Cyan);
                    await CTX.RespondAsync(embed: output.Build());
                }
            }
        }

        [Hidden]
        [Command("twitter")]
        [Aliases("tweet")]
        [Description("Get a random tweet from provided twitter handler")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task CheckTwitter(CommandContext CTX, string query)
        {
            await CTX.RespondAsync($":warning: This command is still in development. Go away!");
            //if (string.IsNullOrWhiteSpace(query))
            //    await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a Twitter handle...");
            //else
            //{
            //    Auth.SetUserCredentials("CONSUMER_KEY", "CONSUMER_SECRET", "ACCESS_TOKEN", "ACCESS_TOKEN_SECRET");
            //    var tweets = Timeline.GetUserTimeline("SimpsonsQOTD", 1);
            //    //Get more control over the request with a UserTimelineParameters
            //    var userTimelineParameters = new UserTimelineParameters();
            //    var tweets = Timeline.GetUserTimeline("SimpsonsQOTD", userTimelineParameters);
            //    var output = new DiscordEmbedBuilder()
            //        .WithColor(DiscordColor.Cyan);
            //    await CTX.RespondAsync(embed: output.Build());
            //    await CTX.TriggerTypingAsync();
            //    APITokenService service = new APITokenService();
            //    string Token = service.GetAPIToken("twitter");
            //    var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, $"https://api.twitter.com/1.1/search/tweets.json?q=%40simpsonsqotd&since_id=24012619984051000&max_id=250126199840518145&result_type=mixed&count=1");
            //    requestUserTimeline.Headers.Add("Authorization", "Bearer " + Token);
            //    //var httpClient = new HttpClient();
            //    using (var http = new HttpClient())
            //    {
            //        await CTX.TriggerTypingAsync();
            //        string response = await http.GetStringAsync(requestUserTimeline.RequestUri);
            //        var tweet = JsonConvert.DeserializeObject<TwitterService.RootObject>(response);
            //        if (tweet == null)
            //            await CTX.RespondAsync("The Twitter you were searching for was not found");
            //        else
            //        {
            //            var output = new DiscordEmbedBuilder()
            //                .WithTitle(tweet.statuses[0].text)
            //                .WithColor(DiscordColor.Cyan);
            //            await CTX.RespondAsync(embed: output.Build());
            //        }
            //    }
            //}
        }

        [Command("twitch")]
        [Aliases("tw")]
        [Description("Get Twitch stream information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task CheckTwitchStream(CommandContext CTX, string stream)
        {
            if (string.IsNullOrWhiteSpace(stream))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a Twitch channel...");
            else
            {
                await CTX.TriggerTypingAsync();
                using (var http = new HttpClient())
                {
                    APITokenService service = new APITokenService();
                    string Token = service.GetAPIToken("twitch");
                    var twitchUrl = $"https://api.twitch.tv/kraken/streams/{stream.ToLower()}?client_id={Token}";
                    string response = await http.GetStringAsync(twitchUrl).ConfigureAwait(false);
                    var twitch = JsonConvert.DeserializeObject<TwitchService>(response);
                    if (twitch.Error != null)
                        await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning: ")} Unable to find this streamer");
                    twitch.Url = twitchUrl;

                    if (twitch.IsLive)
                    {
                        var output = new DiscordEmbedBuilder()
                            .WithTitle(twitch.Title)
                            .AddField("Game", twitch.Game, true)
                            .AddField("Status", twitch.IsLive ? "Online" : "Offline", true)
                            .AddField("Followers", twitch.Followers.ToString(), true)
                            .AddField("Viewers", twitch.Viewers.ToString(), true)
                            .WithThumbnailUrl(twitch.Icon)
                            .WithUrl(twitch.Url);
                        output.WithColor(DiscordColor.Purple);
                        await CTX.RespondAsync(embed: output.Build());
                    }
                    else
                        await CTX.RespondAsync("That Twitch channel is **Offline** :pensive:");
                }
            }
        }

        [Command("weather")]
        [Aliases("we")]
        [Description("Get weather information for specified location")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetWeather(CommandContext CTX, [RemainingText] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please input a location (ex. **.weather Ottawa,CA**)");
            else
            {
                await CTX.TriggerTypingAsync();
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Clear();
                    string response = await http.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q={location}&appid=42cd627dd60debf25a5739e50a217d74&units=metric");
                    var weather = JsonConvert.DeserializeObject<WeatherService.WeatherData>(response);
                    Func<double, double> format = WeatherService.CelsiusToFahrenheit;

                    var output = new DiscordEmbedBuilder()
                        .AddField("Location", $"[{weather.Name + ", " + weather.Sys.Country}](https://openweathermap.org/city/{weather.Id})", true)
                        .AddField("Temperature", $"{weather.Main.Temp:F1}°C / {format(weather.Main.Temp):F1}°F", true)
                        .AddField("Conditions", string.Join(", ", weather.Weather.Select(w => w.Main)), true)
                        .AddField("Humidity", $"{weather.Main.Humidity}%", true)
                        .AddField("Wind Speed", $"{weather.Wind.Speed}m/s", true)
                        .AddField("Temperature (Min/Max)", $"{weather.Main.TempMin:F1}°C - {weather.Main.TempMax:F1}°C\n{format(weather.Main.TempMin):F1}°F - {format(weather.Main.TempMax):F1}°F", true)
                        .WithColor(DiscordColor.Cyan);
                    await CTX.RespondAsync(embed: output.Build());
                }
            }
        }

        [Command("wiki")]
        [Aliases("wikipedia")]
        [Description("Retrieve a wikipedia link")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchWikipedia(CommandContext CTX, [RemainingText] string query)
        {
            await CTX.TriggerTypingAsync();
            using (var http = new HttpClient())
            {
                var result = await http.GetStringAsync("https://en.wikipedia.org//w/api.php?action=query&format=json&prop=info&redirects=1&formatversion=2&inprop=url&titles=" + Uri.EscapeDataString(query));
                var data = JsonConvert.DeserializeObject<WikipediaService>(result);
                if (data.Query.Pages[0].Missing)
                    await CTX.RespondAsync("Wikipedia page not found").ConfigureAwait(false);
                else
                    await CTX.Channel.SendMessageAsync(data.Query.Pages[0].FullUrl).ConfigureAwait(false);
            }
        }
    }
}