using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Services;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models.Impl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OMDbSharp;
using OverwatchAPI;
using PokemonTcgSdk;
using System;
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
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task EightBall(CommandContext ctx, [RemainingText] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                await ctx.RespondAsync(":warning: You have to ask a question! :warning:");
            else
            {
                await ctx.TriggerTypingAsync();
                var rnd = new Random();
                await ctx.RespondAsync($":8ball: {EightBallAnswers.list[rnd.Next(0, EightBallAnswers.list.Count)]}");
            }
        }

        [Command("avatar")]
        [Aliases("av")]
        [Description("Get server user's avatar")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task UserAvatar(CommandContext ctx, [RemainingText] DiscordMember member)
        {
            if (member == null)
                member = ctx.Member;
            await ctx.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"{member.DisplayName}'s avatar, click here for the link...")
                .WithImageUrl(member.GetAvatarUrl(ImageFormat.Jpeg))
                .WithUrl(member.GetAvatarUrl(ImageFormat.Jpeg))
                .WithColor(DiscordColor.Lilac);
            await ctx.RespondAsync(embed: output.Build());
        }

        [Command("catfact")]
        [Aliases("cat")]
        [Description("Get a random cat fact")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task CatFact(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var http = new HttpClient();
            var response = await http.GetStringAsync("https://catfact.ninja/fact");
            await ctx.RespondAsync($":cat: Meow! {JObject.Parse(response)["fact"]}");
        }

        [Command("color")]
        [Description("Get color values corresponding to the inputted RGB")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetColor(CommandContext ctx, params string[] colors)
        {
            try
            {
                await ctx.TriggerTypingAsync();
                var arg = colors[0];
                var rgb = colors.Length == 3;
                var red = Convert.ToByte(rgb ? int.Parse(colors[0]) : Convert.ToInt32(arg.Substring(0, 2), 16));
                var green = Convert.ToByte(rgb ? int.Parse(colors[1]) : Convert.ToInt32(arg.Substring(2, 2), 16));
                var blue = Convert.ToByte(rgb ? int.Parse(colors[2]) : Convert.ToInt32(arg.Substring(4, 2), 16));
                var color = new DiscordColor(red, green, blue);
                var output = new DiscordEmbedBuilder()
                    .AddField("HEX:", $"{color.Value:X}", true)
                    .AddField("RGB:", $"{colors[0]} {colors[1]} {colors[2]}", true)
                    .AddField("Decimal:", $"{color.Value}", true)
                    .WithColor(color);
                await ctx.RespondAsync(embed: output.Build());
            }
            catch
            {
                await ctx.RespondAsync(":warning: Unable to retrieve color values, try **.color [0-255] [0-255] [0-255]** :warning:");
            }
        }

        [Command("define")]
        [Aliases("def")]
        [Description("Get a Dictionary definition for a word or phrase")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Dictionary(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await ctx.RespondAsync(":warning: A word or a phrase is required! :warning:");
            else
            {
                await ctx.TriggerTypingAsync();
                var data = await DefinitionService.GetDefinitionForTermAsync(query);
                if (data.Results.Count == 0)
                    await ctx.RespondAsync(":warning: No results found! :warning:");
                else
                    for (var index = 0; index < data.Results.Count; index++)
                        foreach (var value in data.Results[index].Senses)
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
                                await ctx.RespondAsync(embed: output.Build());

                                var interactivity = await ctx.Client.GetInteractivity()
                                    .WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLower() == "go", TimeSpan.FromSeconds(10));
                                if (interactivity != null) continue;
                                index = data.Results.Count;
                                break;
                            }
            }
        }

        [Command("dictionary")]
        [Aliases("dic")]
        [Description("Get an Urban Dictionary entry for a word or phrase")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task UrbanDictionary(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await ctx.RespondAsync(":warning: A word or a phrase is required! :warning:");
            else
            {
                await ctx.TriggerTypingAsync();
                var data = await DictionaryService.GetDictionaryForTermAsync(query);
                if (data.result_type == "no_results")
                    await ctx.RespondAsync(":warning: No results found! :warning:");
                else
                    foreach (var value in data.list)
                    {
                        var output = new DiscordEmbedBuilder()
                            .WithTitle($"Urban Dictionary definition for **{query}** by {value.author}")
                            .WithDescription(value.definition.Length < 500
                                ? value.definition
                                : value.definition.Take(500) + "...")
                            .WithUrl(value.permalink)
                            .WithFooter("Type next for the next definition")
                            .WithColor(DiscordColor.Blurple);
                        if (!string.IsNullOrWhiteSpace(value.example))
                            output.AddField("Example", value.example);
                        await ctx.RespondAsync(embed: output.Build());

                        var interactivity = await ctx.Client.GetInteractivity()
                            .WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLower() == "next", TimeSpan.FromSeconds(10));
                        if (interactivity == null) break;
                    }
            }
        }

        [Command("imdb")]
        [Aliases("omdb")]
        [Description("Get a movie or TV show from OMDB")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task OMDB(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await ctx.RespondAsync(":warning: A movie or TV show is required! :warning:");
            else
            {
                await ctx.TriggerTypingAsync();
                var service = new APITokenService();
                var token = service.GetAPIToken("omdb");
                var client = new OMDbClient(token, true);
                var movie = await client.GetItemByTitle(query.Replace(" ", "+"));
                if (movie.Response == "False")
                    await ctx.RespondAsync(":warning: No results found! :warning:");
                else
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(movie.Title)
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
                    await ctx.RespondAsync(embed: output.Build());
                }
            }
        }

        [Command("imgur")]
        [Description("Get an imager from Imgur")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Imgur(CommandContext ctx, [RemainingText] string query)
        {
            await ctx.TriggerTypingAsync();
            var rnd = new Random();
            var service = new APITokenService();
            var token = service.GetAPIToken("imgur");
            var imgur = new ImgurClient(token);
            var endpoint = new GalleryEndpoint(imgur);
            var gallery = string.IsNullOrWhiteSpace(query) ? (await endpoint.GetRandomGalleryAsync()).ToList() : (await endpoint.SearchGalleryAsync(query)).ToList();
            var img = gallery.Any() ? gallery[rnd.Next(0, gallery.Count)] : null;
            switch (img)
            {
                case null:
                    await ctx.RespondAsync(":warning: No results found! :warning:");
                    break;

                case GalleryAlbum _:
                    await ctx.RespondAsync(((GalleryAlbum)img).Link);
                    break;

                case GalleryImage _:
                    await ctx.RespondAsync(((GalleryImage)img).Link);
                    break;
            }
        }

        [Command("math")]
        [Description("Perform a basic math operation")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Math(CommandContext ctx, double num1, string operation, double num2)
        {
            try
            {
                double result;
                switch (operation)
                {
                    case "+":
                        result = num1 + num2;
                        break;

                    case "-":
                        result = num1 - num2;
                        break;

                    case "*":
                        result = num1 * num2;
                        break;

                    case "/":
                        result = num1 / num2;
                        break;

                    case "%":
                        result = num1 % num2;
                        break;

                    default:
                        result = num1 + num2;
                        break;
                }

                await ctx.RespondAsync($"{DiscordEmoji.FromName(ctx.Client, ":1234:")} The result is {result:#,##0.00}");
            }
            catch
            {
                await ctx.RespondAsync(":warning: Error calculating math equation, make sure your values are integers and the operation is valid! :warning:");
            }
        }

        [Command("overwatch")]
        [Aliases("ow")]
        [Description("Get Overwatch player information")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Overwatch(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await ctx.RespondAsync(":warning: Blizzard Battletag is required! Try **.ow CriticalFlaw#11354** (Case-sensitive) :warning:");
            else
            {
                await ctx.TriggerTypingAsync();
                var overwatch = new OverwatchClient();
                var player = await overwatch.GetPlayerAsync(query);
                if (player == null)
                    await ctx.RespondAsync(":warning: Player not found! Note; battletags are case-sensitive :warning:");
                else
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(player.Username)
                        .AddField("Level", player.PlayerLevel.ToString(), true)
                        .AddField("Competitive", player.CompetitiveRank.ToString(), true)
                        .AddField("Platform", player.Platform.ToString().ToUpper(), true)
                        .AddField("Achievements", player.Achievements.Count.ToString(), true)
                        .WithThumbnailUrl(player.ProfilePortraitUrl)
                        .WithUrl(player.ProfileUrl)
                        .WithColor(DiscordColor.Gold);
                    await ctx.RespondAsync(embed: output.Build());
                }
            }
        }

        [Command("pokemon")]
        [Aliases("poke")]
        [Description("Get Pokemon information")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Pokemon(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await ctx.RespondAsync(":warning: Pokemon name is required! Try **.poke charizard** :warning:");
            else
            {
                await ctx.TriggerTypingAsync();
                var pokemon = await PokemonService.GetPokemonDataAsync(query);
                if (pokemon.cards.Count == 0)
                    await ctx.RespondAsync(":warning: Unable to find this Pokemon");
                else
                {
                    var rnd = new Random();
                    var cards = Card.Find<Pokemon>(pokemon.cards[rnd.Next(0, pokemon.cards.Count)].id);
                    var output = new DiscordEmbedBuilder()
                        .WithTitle($"{cards.Card.Name} (ID: {cards.Card.NationalPokedexNumber})")
                        .WithImageUrl(cards.Card.ImageUrl)
                        .WithColor(DiscordColor.Lilac)
                        .WithFooter($"Card ID: {cards.Card.Id}");
                    await ctx.RespondAsync(embed: output.Build());
                }
            }
        }

        [Command("randomcat")]
        [Aliases("meow")]
        [Description("Get a random cat image")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task RandomCat(CommandContext ctx)
        {
            using (var client = new WebClient())
            {
                await ctx.TriggerTypingAsync();
                var image = client.DownloadString("http://random.cat/meow");
                var iFrom = image.IndexOf("\\/i\\/", StringComparison.Ordinal) + "\\/i\\/".Length;
                var iTo = image.LastIndexOf("\"}", StringComparison.Ordinal);
                await ctx.RespondAsync($":cat: Meow! http://random.cat/i/{image.Substring(iFrom, iTo - iFrom)}");
            }
        }

        [Command("randomdog")]
        [Aliases("woof")]
        [Description("Get a random dog image")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task RandomDog(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            using (var client = new WebClient())
                await ctx.RespondAsync($":dog: Bork! http://random.dog/{client.DownloadString("http://random.dog/woof")}");
        }

        [Command("simpsons")]
        [Aliases("doh")]
        [Description("Get a random Simpsons screenshot and episode")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SimpsonsEpisode(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var data = await SimpsonsService.GetSimpsonsDataAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle(data.Episode.Title)
                .AddField("Season/Episode", data.Episode.Key, true)
                .AddField("Air Date", data.Episode.OriginalAirDate, true)
                .AddField("Writer", data.Episode.Writer, true)
                .AddField("Director", data.Episode.Director, true)
                .WithImageUrl($"https://frinkiac.com/img/{data.Frame.Episode}/{data.Frame.Timestamp}.jpg")
                .WithUrl(data.Episode.WikiLink)
                .WithColor(DiscordColor.Yellow);
            await ctx.RespondAsync(embed: output.Build());
        }

        [Command("simpsonsgif")]
        [Description("Get a random Simpsons gif")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SimpsonsGif(CommandContext ctx, [RemainingText] string input)
        {
            await ctx.TriggerTypingAsync();
            var gif = await SimpsonsService.GetSimpsonsGifAsync();
            if (string.IsNullOrWhiteSpace(input))
                await ctx.RespondAsync(gif);
            else // Include episode information if any kind of parameter is inputted
            {
                var data = await SimpsonsService.GetSimpsonsDataAsync();
                var output = new DiscordEmbedBuilder()
                    .WithTitle(data.Episode.Title)
                    .AddField("Season/Episode", data.Episode.Key, true)
                    .AddField("Air Date", data.Episode.OriginalAirDate, true)
                    .AddField("Writer", data.Episode.Writer, true)
                    .AddField("Director", data.Episode.Director, true)
                    .WithFooter("Note: First time gifs take a few minutes to properly generate")
                    .WithUrl(data.Episode.WikiLink)
                    .WithColor(DiscordColor.Yellow);
                await ctx.RespondAsync(gif, embed: output.Build());
            }
        }

        [Command("sum")]
        [Aliases("total")]
        [Description("Sum all inputted numbers")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SumOfNumbers(CommandContext ctx, params int[] args)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($":1234: The sum is {args.Sum():#,##0}");
        }

        [Command("time")]
        [Aliases("ti")]
        [Description("Get time for specified location")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetTime(CommandContext ctx, [RemainingText] string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location))
                    await ctx.RespondAsync(":warning: A valid location is required! Try **.time Ottawa, CA** :warning:");
                else
                {
                    await ctx.TriggerTypingAsync();
                    var http = new HttpClient();
                    http.DefaultRequestHeaders.Clear();
                    var service = new APITokenService();
                    var token = service.GetAPIToken("google");
                    var locationResource = await http.GetStringAsync($"https://maps.googleapis.com/maps/api/geocode/json?address={location.Replace(" ", "")}&key={token}");
                    var locationObject = JsonConvert.DeserializeObject<TimeService>(locationResource);
                    if (locationObject.status != "OK")
                        await ctx.RespondAsync(":warning: Unable to find this location! :warning:");
                    else
                    {
                        var currentSeconds = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                        var url = $"https://maps.googleapis.com/maps/api/timezone/json?location={locationObject.results[0].geometry.location.lat},{locationObject.results[0].geometry.location.lng}&timestamp={currentSeconds}&key={token}";
                        var timeResource = await http.GetStringAsync(url);
                        var timeObject = JsonConvert.DeserializeObject<TimeService.TimeZoneResult>(timeResource);
                        var time = DateTime.UtcNow.AddSeconds(timeObject.dstOffset + timeObject.rawOffset);
                        var output = new DiscordEmbedBuilder()
                            .WithTitle($"Time in {locationObject.results[0].formatted_address}")
                            .WithDescription($":clock1: **{time.ToShortTimeString()}** {timeObject.timeZoneName}")
                            .WithColor(DiscordColor.Cyan);
                        await ctx.RespondAsync(embed: output.Build());
                    }
                }
            }
            catch
            {
                await ctx.RespondAsync(":warning: Unable to find time data for this location! :warning:");
            }
        }

        [Command("twitch")]
        [Aliases("tw")]
        [Description("Get Twitch stream information")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task CheckTwitchStream(CommandContext ctx, [RemainingText] string stream)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stream))
                    await ctx.RespondAsync(":warning: A valid Twitch channel name is required! :warning:");
                else
                {
                    await ctx.TriggerTypingAsync();
                    var http = new HttpClient();
                    var service = new APITokenService();
                    var token = service.GetAPIToken("twitch");
                    var twitchUrl = $"https://api.twitch.tv/kraken/streams/{stream.ToLower()}?client_id={token}";
                    var response = await http.GetStringAsync(twitchUrl).ConfigureAwait(false);
                    var twitch = JsonConvert.DeserializeObject<TwitchService>(response);
                    twitch.Url = twitchUrl;
                    if (!twitch.IsLive)
                        await ctx.RespondAsync("That Twitch channel is **Offline** (or doesn't exist) :pensive:");
                    else
                    {
                        var output = new DiscordEmbedBuilder()
                            .WithTitle($"{twitch.Stream.Channel.display_name} is live streaming on Twitch!")
                            .AddField("Now Playing", twitch.Game)
                            .AddField("Stream Title", twitch.Title)
                            .AddField("Followers", twitch.Followers.ToString(), true)
                            .AddField("Viewers", twitch.Viewers.ToString(), true)
                            .WithThumbnailUrl(twitch.Icon)
                            .WithUrl(twitch.Url)
                            .WithColor(DiscordColor.Purple);
                        await ctx.RespondAsync(embed: output.Build());
                    }
                }
            }
            catch
            {
                await ctx.RespondAsync(":warning: Error processing channel status, please do not include special characters in your input! :warning:");
            }
        }

        [Command("weather")]
        [Aliases("we")]
        [Description("Get weather information for specified location")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetWeather(CommandContext ctx, [RemainingText] string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location))
                    await ctx.RespondAsync(":warning: A valid location is required! Try **.weather Ottawa, CA** :warning:");
                else
                {
                    await ctx.TriggerTypingAsync();
                    var http = new HttpClient();
                    http.DefaultRequestHeaders.Clear();
                    var response = await http.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q={location}&appid=42cd627dd60debf25a5739e50a217d74&units=metric");
                    var weather = JsonConvert.DeserializeObject<WeatherService.WeatherData>(response);
                    if (weather.cod == 404)
                        await ctx.RespondAsync(":warning: Unable to find this location! :warning:");
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
                        await ctx.RespondAsync(embed: output.Build());
                    }
                }
            }
            catch
            {
                await ctx.RespondAsync(":warning: Unable to find weather data for this location! :warning:");
            }
        }

        [Command("wiki")]
        [Aliases("wikipedia")]
        [Description("Retrieve a wikipedia link")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchWikipedia(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await ctx.RespondAsync(":warning: Wikipedia search query is required! :warning:");
            else
            {
                await ctx.TriggerTypingAsync();
                var http = new HttpClient();
                var result = await http.GetStringAsync($"https://en.wikipedia.org//w/api.php?action=query&format=json&prop=info&redirects=1&formatversion=2&inprop=url&titles={Uri.EscapeDataString(query)}");
                var data = JsonConvert.DeserializeObject<WikipediaService>(result);
                if (data.Query.Pages[0].Missing)
                    await ctx.RespondAsync(":warning: Unable to find this Wikipedia page! :warning:");
                else
                    await ctx.Channel.SendMessageAsync(data.Query.Pages[0].FullUrl).ConfigureAwait(false);
            }
        }
    }
}