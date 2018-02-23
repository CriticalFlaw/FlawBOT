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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
        [Description("Retrieve mentioned user's avatar")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchUserAvatar(CommandContext CTX, DiscordMember member)
        {
            if (member is DiscordMember)
            {
                await CTX.TriggerTypingAsync();
                var output = new DiscordEmbedBuilder()
                    .WithTitle($"{member.DisplayName}'s avatar, click here for the link")
                    .WithImageUrl($"{member.GetAvatarUrl(ImageFormat.Jpeg)}")
                    .WithUrl($"{member.GetAvatarUrl(ImageFormat.Jpeg)}")
                    .WithColor(DiscordColor.Blue);
                await CTX.RespondAsync(embed: output.Build());
            }
            else
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} This user does not exist in the server");
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
        [Description("Retrieve color values corresponding to inputted RGB")]
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
                    .AddField("**RGB:**", $"{colors}")
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
                    .WithColor(DiscordColor.Lilac);

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
        [Description("Retrieve a movie or TV show from OMDB")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchIMDB(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a movie or TV show to search for...");
            else
            {
                await CTX.TriggerTypingAsync();
                var JSON = "";  // Load the configuration file
                using (var SRD = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)))
                    JSON = await SRD.ReadToEndAsync();
                string Token = JsonConvert.DeserializeObject<APITokenService.APITokenList>(JSON).OMDBToken;
                OMDbClient client = new OMDbClient(Token, true);
                var movie = await client.GetItemByTitle(query.Replace(" ", "+"));
                if (movie == null)
                    await CTX.RespondAsync("The movie you were searching for was not found");
                else
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(movie.Title.ToString())
                        .WithDescription(movie.Plot.Length < 500 ? movie.Plot : movie.Plot.Take(500) + "...")
                        .WithImageUrl(movie.Poster)
                        .AddField("Genre", movie.Genre, true)
                        .AddField("Released", movie.Released, true)
                        .AddField("Runtime", movie.Runtime, true)
                        .AddField("Production", movie.Production, true)
                        .AddField("Country", movie.Country, true)
                        .AddField("BoxOffice", movie.BoxOffice, true)
                        .AddField("Director", movie.Director, true)
                        .AddField("Actors", movie.Actors, true)
                        .AddField("IMDB Rating", movie.imdbRating, true)
                        .AddField("Metacritic", movie.Metascore, true)
                        .AddField("RottenTomato", movie.tomatoRating, true)
                        .WithColor(DiscordColor.CornflowerBlue);
                    await CTX.RespondAsync(embed: output.Build());
                }
            }
        }

        [Command("imgur")]
        [Description("Fetch an imager from Imgur")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Find(CommandContext CTX, [RemainingText] string query = null)
        {
            await CTX.TriggerTypingAsync();
            Random RND = new Random();
            var JSON = "";  // Load the configuration file
            using (var SRD = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)))
                JSON = await SRD.ReadToEndAsync();
            string Token = JsonConvert.DeserializeObject<APITokenService.APITokenList>(JSON).ImgurToken;
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

        [Hidden]
        [Command("overwatch")]
        [Aliases("ow")]
        [Description("Get Overwatch player information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchOverwatch(CommandContext CTX, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a battletag (ex.*CriticalFlaw#1100*)");
            else
            {
                await CTX.TriggerTypingAsync();
                //using (var OW = new OverwatchClient())
                //{
                //    Player player = await OW.GetPlayerAsync(query);
                //    if (player == null)
                //        await CTX.RespondAsync($"The player you were searching for was not found");
                //    else
                //    {
                //        var output = new DiscordEmbedBuilder()
                //            .WithTitle(player.Username)
                //            .AddField("Level", player.PlayerLevel.ToString(), true)
                //            .AddField("Competitive", player.CompetitiveRank.ToString(), true)
                //            .AddField("Platform", player.Platform.ToString().ToUpper(), true)
                //            .WithThumbnailUrl(player.ProfilePortraitUrl)
                //            .WithUrl(player.ProfileUrl)
                //            .WithColor(DiscordColor.Gold);
                //        await CTX.RespondAsync(embed: output.Build());
                //    }
                //}
            }
        }

        [Command("randomcat")]
        [Aliases("meow")]
        [Description("Retrieve a random cat image")]
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
        [Description("Retrieve a random dog image")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task RandomDog(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            using (WebClient client = new WebClient())
                await CTX.RespondAsync(":dog: Bork! http://random.dog/" + client.DownloadString("http://random.dog/woof"));
        }

        [Command("simpsons")]
        [Aliases("doh")]
        [Description("Get a random Simpsons screenshot")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchSimpsonsScreenshot(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            SimpsonsService.RootObject data = new SimpsonsService.RootObject();
            data = await SimpsonsService.GetSimpsonsDataAsync();
            await CTX.RespondAsync($"https://frinkiac.com/img/{data.Frame.Episode}/{data.Frame.Timestamp}.jpg");
        }

        [Command("simpsonsepisode")]
        [Description("Get a random Simpsons episode")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchSimpsonsEpisode(CommandContext CTX)
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
                .WithUrl(data.Episode.WikiLink)
                .WithColor(DiscordColor.Yellow);
            await CTX.RespondAsync(embed: output.Build());
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
        [Description("Retrieve time and timezone in the specified location")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetTime(CommandContext CTX, [RemainingText] string location)
        {
            await CTX.TriggerTypingAsync();
            var JSON = "";  // Load the configuration file
            using (var SRD = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)))
                JSON = await SRD.ReadToEndAsync();
            string Token = JsonConvert.DeserializeObject<APITokenService.APITokenList>(JSON).GoogleToken;

            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Clear();
                var locationResource = await http.GetStringAsync($"https://maps.googleapis.com/maps/api/geocode/json?address={location.Replace(" ", "")}&key={Token}");
                var locationObject = JsonConvert.DeserializeObject<TimeService>(locationResource);
                var currentSeconds = DateTime.UtcNow.ToUniversalTime();
                var timeResource = await http.GetStringAsync($"https://maps.googleapis.com/maps/api/timezone/json?location={locationObject.results[0].Geometry.Location.Lat},{locationObject.results[0].Geometry.Location.Lng}&timestamp={currentSeconds}&key={Token}");
                var timeObject = JsonConvert.DeserializeObject<TimeService.TimeZoneResult>(timeResource);
                var time = DateTime.UtcNow.AddSeconds(timeObject.DstOffset + timeObject.RawOffset);

                var output = new DiscordEmbedBuilder()
                    .WithTitle($"Time in {location}")
                    .WithDescription($"{DiscordEmoji.FromName(CTX.Client, ":clock1:")}  **{time.ToShortTimeString()} UTC**")
                    .WithColor(DiscordColor.Cyan);
                await CTX.RespondAsync(embed: output.Build());
            }
        }

        [Command("twitch")]
        [Aliases("tw")]
        [Description("Check Twtich stream status, return URL if online")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task CheckTwitchStream(CommandContext CTX, string stream)
        {
            await CTX.TriggerTypingAsync();
            using (var http = new HttpClient())
            {
                var twitchUrl = $"https://api.twitch.tv/kraken/streams/{stream.ToLower()}?client_id=67w6z9i09xv2uoojdm9l0wsyph4hxo6";
                string response = await http.GetStringAsync(twitchUrl).ConfigureAwait(false);
                var twitch = JsonConvert.DeserializeObject<TwitchService>(response);
                if (twitch.Error != null)
                    await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning: ")} Unable to find this streamer");
                twitch.Url = twitchUrl;

                var output = new DiscordEmbedBuilder()
                    .WithTitle(twitch.Title)
                    .AddField("Game", twitch.Game, true)
                    .AddField("Status", twitch.IsLive ? "Online" : "Offline", true)
                    .AddField("Followers", twitch.Followers.ToString(), true)
                    .AddField("Viewers", twitch.Viewers.ToString(), true)
                    .WithThumbnailUrl(twitch.Icon)
                    .WithUrl(twitch.Url);
                if (twitch.Live == false)
                    output.WithColor(DiscordColor.Red);
                else
                    output.WithColor(DiscordColor.Purple);
                await CTX.RespondAsync(embed: output.Build());
            }
        }

        [Command("weather")]
        [Aliases("we")]
        [Description("Retrieve weather data for the specified city")]
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