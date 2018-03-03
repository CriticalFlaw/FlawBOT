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
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task EightBall(CommandContext CTX, [RemainingText] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                await CTX.RespondAsync(":warning: You have to ask a question! :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                Random RND = new Random();
                List<string> EightBallAnswers = CommonService.Get8BallList();
                await CTX.RespondAsync(EightBallAnswers[RND.Next(0, EightBallAnswers.Count)]);
            }
        }

        [Command("avatar")]
        [Aliases("av")]
        [Description("Get server user's avatar")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task UserAvatar(CommandContext CTX, [RemainingText] DiscordMember member)
        {
            if (member == null)
                member = CTX.Member;
            await CTX.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"{member.DisplayName}'s avatar, click here for the link...")
                .WithImageUrl($"{member.GetAvatarUrl(ImageFormat.Jpeg)}")
                .WithUrl($"{member.GetAvatarUrl(ImageFormat.Jpeg)}")
                .WithColor(DiscordColor.Lilac);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("catfact")]
        [Aliases("cat")]
        [Description("Get a random cat fact")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task CatFact(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            HttpClient http = new HttpClient();
            var response = await http.GetStringAsync("https://catfact.ninja/fact");
            await CTX.RespondAsync($":cat: Meow! {JObject.Parse(response)["fact"].ToString()}");
        }

        [Command("color")]
        [Description("Get color values corresponding to the inputted RGB")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task GetColor(CommandContext CTX, params string[] colors)
        {
            try
            {
                await CTX.TriggerTypingAsync();
                var ARG = colors[0];
                var RGB = colors.Length == 3;
                var RED = Convert.ToByte(RGB ? int.Parse(colors[0]) : Convert.ToInt32(ARG.Substring(0, 2), 16));
                var GREEN = Convert.ToByte(RGB ? int.Parse(colors[1]) : Convert.ToInt32(ARG.Substring(2, 2), 16));
                var BLUE = Convert.ToByte(RGB ? int.Parse(colors[2]) : Convert.ToInt32(ARG.Substring(4, 2), 16));
                DiscordColor color = new DiscordColor(RED, GREEN, BLUE);
                var output = new DiscordEmbedBuilder()
                    .AddField("**HEX:**", $"{color.Value.ToString("X")}", true)
                    .AddField("**RGB:**", $"{colors[0]} {colors[1]} {colors[2]}", true)
                    .AddField("**Decimal:**", $"{color.Value}", true)
                    .WithColor(color);
                await CTX.RespondAsync(embed: output.Build());
            }
            catch
            {
                await CTX.RespondAsync(":warning: Unable to retrieve color values, try **.color [0-255] [0-255] [0-255]** :warning:");
            }
        }

        [Command("define")]
        [Aliases("def")]
        [Description("Get a Dictionary definition for a word or phrase")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task Dictionary(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync(":warning: A word or a phrase is required! :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                DefinitionService.Data data = new DefinitionService.Data();
                data = await DefinitionService.GetDefinitionForTermAsync(query);
                if (data.Results.Count == 0)
                    await CTX.RespondAsync(":warning: No results found! :warning:");
                else
                {
                    for (int index = 0; index < data.Results.Count; index++)
                    {
                        foreach (var value in data.Results[index].Senses)
                        {
                            if (value.Definition != null)
                            {
                                var definition = value.Definition.ToString();
                                if (!(value.Definition is string))
                                    definition = ((JArray)JToken.Parse(value.Definition.ToString())).First.ToString();
                                var output = new DiscordEmbedBuilder()
                                    .WithTitle($"Dictionary definition for **{query}**")
                                    .WithDescription(definition.Length < 500 ? definition : definition.Take(500) + "...")
                                    .WithFooter("Type go for the next definition")
                                    .WithColor(DiscordColor.Blurple);
                                if (value.Examples != null)
                                    output.AddField("Example", value.Examples.First().text);
                                await CTX.RespondAsync(embed: output.Build());

                                var interactivity = await CTX.Client.GetInteractivityModule().WaitForMessageAsync(m => m.Channel.Id == CTX.Channel.Id && m.Content.ToLower() == "go", TimeSpan.FromSeconds(10));
                                if (interactivity == null)
                                {
                                    index = data.Results.Count;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        [Command("dictionary")]
        [Aliases("dic")]
        [Description("Get an Urban Dictionary entry for a word or phrase")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task UrbanDictionary(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync(":warning: A word or a phrase is required! :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                DictionaryService.RootObject data = new DictionaryService.RootObject();
                data = await DictionaryService.GetDictionaryForTermAsync(query);
                if (data.result_type == "no_results")
                    await CTX.RespondAsync(":warning: No results found! :warning:");
                else
                {
                    foreach (var value in data.list)
                    {
                        var output = new DiscordEmbedBuilder()
                            .WithTitle($"Urban Dictionary definition for **{query}** by {value.author}")
                            .WithDescription(value.definition.Length < 500 ? value.definition : value.definition.Take(500) + "...")
                            .WithUrl(value.permalink)
                            .WithFooter("Type next for the next definition")
                            .WithColor(DiscordColor.Blurple);
                        if (!string.IsNullOrWhiteSpace(value.example))
                            output.AddField("Example", value.example);
                        await CTX.RespondAsync(embed: output.Build());

                        var interactivity = await CTX.Client.GetInteractivityModule().WaitForMessageAsync(m => m.Channel.Id == CTX.Channel.Id && m.Content.ToLower() == "next", TimeSpan.FromSeconds(10));
                        if (interactivity == null) break;
                    }
                }
            }
        }

        [Command("imdb")]
        [Aliases("omdb")]
        [Description("Get a movie or TV show from OMDB")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task OMDB(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync(":warning: A movie or TV show is required! :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                APITokenService service = new APITokenService();
                string Token = service.GetAPIToken("omdb");
                OMDbClient client = new OMDbClient(Token, true);
                var movie = await client.GetItemByTitle(query.Replace(" ", "+"));
                if (movie.Response == "False")
                    await CTX.RespondAsync(":warning: No results found! :warning:");
                else
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(movie.Title.ToString())
                        .WithDescription(movie.Plot.Length < 500 ? movie.Plot : movie.Plot.Take(500) + "...")
                        .AddField("Released", movie.Released, true)
                        .AddField("Runtime", movie.Runtime, true)
                        .AddField("Genre", movie.Genre, true)
                        .AddField("Country", movie.Country, true)
                        .AddField("Box Office", movie.BoxOffice, true)
                        .AddField("Production", movie.Production, true)
                        .AddField("IMDB Rating", movie.imdbRating, true)
                        .AddField("Metacritic", movie.Metascore, true)
                        .AddField("Rotten Tomatoes", movie.tomatoRating, true)
                        .AddField("Director", movie.Director, true)
                        .AddField("Actors", movie.Actors, true)
                        .WithColor(DiscordColor.Goldenrod);
                    if (movie.Poster != "N/A") output.WithImageUrl(movie.Poster);
                    if (movie.tomatoURL != "N/A") output.WithUrl(movie.tomatoURL);
                    await CTX.RespondAsync(embed: output.Build());
                }
            }
        }

        [Command("imgur")]
        [Description("Get an imager from Imgur")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task Imgur(CommandContext CTX, [RemainingText] string query)
        {
            await CTX.TriggerTypingAsync();
            Random RND = new Random();
            APITokenService service = new APITokenService();
            string Token = service.GetAPIToken("imgur");
            ImgurClient imgur = new ImgurClient(Token);
            var endpoint = new GalleryEndpoint(imgur);
            List<IGalleryItem> gallery;
            if (string.IsNullOrWhiteSpace(query))
                gallery = (await endpoint.GetRandomGalleryAsync()).ToList();
            else
                gallery = (await endpoint.SearchGalleryAsync(query)).ToList();
            var IMG = gallery.Any() ? gallery[RND.Next(0, gallery.Count)] : null;
            if (IMG == null)
                await CTX.RespondAsync(":warning: No results found! :warning:");
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
        [Cooldown(3, 5, CooldownBucketType.User)]
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

        [Command("overwatch")]
        [Aliases("ow")]
        [Description("Get Overwatch player information")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task Overwatch(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync(":warning: Blizzard Battletag is required! Try **.ow CriticalFlaw#11354** (Case-sensitive) :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                OverwatchClient overwatch = new OverwatchClient();
                Player player = await overwatch.GetPlayerAsync(query);
                if (player == null)
                    await CTX.RespondAsync(":warning: Player not found! Note; battletags are case-sensitive :warning:");
                else
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(player.Username)
                        .AddField("Level", player.PlayerLevel.ToString(), true)
                        .AddField("Competitive", player.CompetitiveRank.ToString(), true)
                        .AddField("Platform", player.Platform.ToString().ToUpper(), true)
                        .AddField("Achievements", player.Achievements.Count().ToString(), true)
                        .WithThumbnailUrl(player.ProfilePortraitUrl)
                        .WithUrl(player.ProfileUrl)
                        .WithColor(DiscordColor.Gold);
                    await CTX.RespondAsync(embed: output.Build());
                }
            }
        }

        [Command("pokemon")]
        [Aliases("poke")]
        [Description("Get Pokemon information")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task Pokemon(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync(":warning: Pokemon name is required! Try **.poke charizard** :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                PokemonService.RootObject pokemon = new PokemonService.RootObject();
                pokemon = await PokemonService.GetPokemonDataAsync(query);
                if (pokemon.cards.Count == 0)
                    await CTX.RespondAsync(":warning: Unable to find this Pokemon");
                else
                {
                    Random RNG = new Random();
                    var cards = Card.Find<Pokemon>(pokemon.cards[RNG.Next(0, pokemon.cards.Count)].id);
                    var output = new DiscordEmbedBuilder()
                        .WithTitle($"{cards.Card.Name} (ID: {cards.Card.NationalPokedexNumber})")
                        .WithImageUrl(cards.Card.ImageUrl)
                        .WithColor(DiscordColor.Lilac)
                        .WithFooter($"Card ID: {cards.Card.Id}");
                    await CTX.RespondAsync(embed: output.Build());
                }
            }
        }

        [Command("randomcat")]
        [Aliases("meow")]
        [Description("Get a random cat image")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task RandomCat(CommandContext CTX)
        {
            using (WebClient client = new WebClient())
            {
                await CTX.TriggerTypingAsync();
                var image = client.DownloadString("http://random.cat/meow");
                int iFrom = image.IndexOf("\\/i\\/") + "\\/i\\/".Length;
                int iTo = image.LastIndexOf("\"}");
                await CTX.RespondAsync($":cat: Meow! http://random.cat/i/{image.Substring(iFrom, iTo - iFrom)}");
            }
        }

        [Command("randomdog")]
        [Aliases("woof")]
        [Description("Get a random dog image")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task RandomDog(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            using (WebClient client = new WebClient())
                await CTX.RespondAsync($":dog: Bork! http://random.dog/{client.DownloadString("http://random.dog/woof")}");
        }

        [Command("simpsons")]
        [Aliases("doh")]
        [Description("Get a random Simpsons screenshot and episode")]
        [Cooldown(3, 5, CooldownBucketType.User)]
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
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task SimpsonsGif(CommandContext CTX, [RemainingText] string input)
        {
            await CTX.TriggerTypingAsync();
            var gif = await SimpsonsService.GetSimpsonsGifAsync();
            if (string.IsNullOrWhiteSpace(input))
                await CTX.RespondAsync(gif.ToString());
            else // Include episode information if any kind of parameter is inputted
            {
                SimpsonsService.RootObject data = new SimpsonsService.RootObject();
                data = await SimpsonsService.GetSimpsonsDataAsync();
                var output = new DiscordEmbedBuilder()
                    .WithTitle(data.Episode.Title)
                    .AddField("Season/Episode", data.Episode.Key, true)
                    .AddField("Air Date", data.Episode.OriginalAirDate, true)
                    .AddField("Writer", data.Episode.Writer, true)
                    .AddField("Director", data.Episode.Director, true)
                    .WithFooter("Note: First time gifs take a few minutes to properly generate")
                    .WithUrl(data.Episode.WikiLink)
                    .WithColor(DiscordColor.Yellow);
                await CTX.RespondAsync(gif.ToString(), embed: output.Build());
            }
        }

        [Command("sum")]
        [Aliases("total")]
        [Description("Sum all inputted numbers")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task SumOfNumbers(CommandContext CTX, params int[] args)
        {
            await CTX.TriggerTypingAsync();
            await CTX.RespondAsync($":1234: The sum is {args.Sum().ToString("#,##0")}");
        }

        [Command("time")]
        [Aliases("ti")]
        [Description("Get time for specified location")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task GetTime(CommandContext CTX, [RemainingText] string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location))
                    await CTX.RespondAsync(":warning: A valid location is required! Try **.time Ottawa, CA** :warning:");
                else
                {
                    await CTX.TriggerTypingAsync();
                    HttpClient http = new HttpClient();
                    http.DefaultRequestHeaders.Clear();
                    APITokenService service = new APITokenService();
                    string Token = service.GetAPIToken("google");
                    var locationResource = await http.GetStringAsync($"https://maps.googleapis.com/maps/api/geocode/json?address={location.Replace(" ", "")}&key={Token}");
                    var locationObject = JsonConvert.DeserializeObject<TimeService>(locationResource);
                    if (locationObject.status != "OK")
                        await CTX.RespondAsync(":warning: Unable to find this location! :warning:");
                    else
                    {
                        var currentSeconds = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        var url = $"https://maps.googleapis.com/maps/api/timezone/json?location={locationObject.results[0].geometry.location.lat},{locationObject.results[0].geometry.location.lng}&timestamp={currentSeconds}&key={Token}";
                        var timeResource = await http.GetStringAsync(url);
                        var timeObject = JsonConvert.DeserializeObject<TimeService.TimeZoneResult>(timeResource);
                        var time = DateTime.UtcNow.AddSeconds(timeObject.dstOffset + timeObject.rawOffset);
                        var output = new DiscordEmbedBuilder()
                            .WithTitle($"Time in {locationObject.results[0].formatted_address}")
                            .WithDescription($":clock1: **{time.ToShortTimeString()}** {timeObject.timeZoneName}")
                            .WithColor(DiscordColor.Cyan);
                        await CTX.RespondAsync(embed: output.Build());
                    }
                }
            }
            catch
            {
                await CTX.RespondAsync(":warning: Unable to find time data for this location! :warning:");
            }
        }

        [Command("twitch")]
        [Aliases("tw")]
        [Description("Get Twitch stream information")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task CheckTwitchStream(CommandContext CTX, [RemainingText] string stream)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stream))
                    await CTX.RespondAsync(":warning: A valid Twitch channel name is required! :warning:");
                else
                {
                    await CTX.TriggerTypingAsync();
                    HttpClient http = new HttpClient();
                    APITokenService service = new APITokenService();
                    string Token = service.GetAPIToken("twitch");
                    var twitchUrl = $"https://api.twitch.tv/kraken/streams/{stream.ToLower()}?client_id={Token}";
                    string response = await http.GetStringAsync(twitchUrl).ConfigureAwait(false);
                    var twitch = JsonConvert.DeserializeObject<TwitchService>(response);
                    twitch.Url = twitchUrl;
                    if (!twitch.IsLive)
                        await CTX.RespondAsync("That Twitch channel is **Offline** (or doesn't exist) :pensive:");
                    else
                    {
                        var output = new DiscordEmbedBuilder()
                            .WithTitle($"{twitch.stream.channel.display_name} is live streaming on Twitch!")
                            .AddField("Now Playing", twitch.Game)
                            .AddField("Stream Title", twitch.Title)
                            .AddField("Followers", twitch.Followers.ToString(), true)
                            .AddField("Viewers", twitch.Viewers.ToString(), true)
                            .WithThumbnailUrl(twitch.Icon)
                            .WithUrl(twitch.Url)
                            .WithColor(DiscordColor.Purple);
                        await CTX.RespondAsync(embed: output.Build());
                    }
                }
            }
            catch
            {
                await CTX.RespondAsync(":warning: Error processing channel status, please do not include special characters in your input! :warning:");
            }
        }

        [Command("weather")]
        [Aliases("we")]
        [Description("Get weather information for specified location")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task GetWeather(CommandContext CTX, [RemainingText] string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location))
                    await CTX.RespondAsync(":warning: A valid location is required! Try **.weather Ottawa, CA** :warning:");
                else
                {
                    await CTX.TriggerTypingAsync();
                    HttpClient http = new HttpClient();
                    http.DefaultRequestHeaders.Clear();
                    string response = await http.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q={location}&appid=42cd627dd60debf25a5739e50a217d74&units=metric");
                    var weather = JsonConvert.DeserializeObject<WeatherService.WeatherData>(response);
                    if (weather.cod == 404)
                        await CTX.RespondAsync(":warning: Unable to find this location! :warning:");
                    else
                    {
                        Func<double, double> format = WeatherService.CelsiusToFahrenheit;
                        var output = new DiscordEmbedBuilder()
                            .AddField("Location", $"[{weather.name + ", " + weather.sys.country}](https://openweathermap.org/city/{weather.id})", true)
                            .AddField("Temperature", $"{weather.main.temp:F1}°C / {format(weather.main.temp):F1}°F", true)
                            .AddField("Conditions", string.Join(", ", weather.weather.Select(w => w.main)), true)
                            .AddField("Humidity", $"{weather.main.humidity}%", true)
                            .AddField("Wind Speed", $"{weather.wind.speed}m/s", true)
                            .AddField("Temperature (Min/Max)", $"{weather.main.tempMin:F1}°C - {weather.main.tempMax:F1}°C\n{format(weather.main.tempMin):F1}°F - {format(weather.main.tempMax):F1}°F", true)
                            .WithColor(DiscordColor.Cyan);
                        await CTX.RespondAsync(embed: output.Build());
                    }
                }
            }
            catch
            {
                await CTX.RespondAsync(":warning: Unable to find weather data for this location! :warning:");
            }
        }

        [Command("wiki")]
        [Aliases("wikipedia")]
        [Description("Retrieve a wikipedia link")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task SearchWikipedia(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync(":warning: Wikipedia search query is required! :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                HttpClient http = new HttpClient();
                var result = await http.GetStringAsync($"https://en.wikipedia.org//w/api.php?action=query&format=json&prop=info&redirects=1&formatversion=2&inprop=url&titles={Uri.EscapeDataString(query)}");
                var data = JsonConvert.DeserializeObject<WikipediaService>(result);
                if (data.Query.Pages[0].Missing)
                    await CTX.RespondAsync(":warning: Unable to find this Wikipedia page! :warning:");
                else
                    await CTX.Channel.SendMessageAsync(data.Query.Pages[0].FullUrl).ConfigureAwait(false);
            }
        }
    }
}