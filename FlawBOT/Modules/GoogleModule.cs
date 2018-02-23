using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Services;
using Google.Apis.Services;
using Google.Apis.Urlshortener.v1;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class GoogleModule
    {
        [Hidden]
        [Command("google")]
        [Aliases("g")]
        [Description("Retrieve the first Google search result")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchGoogle(CommandContext CTX, [RemainingText] string terms = null)
        {
            await CTX.TriggerTypingAsync();
            await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} This command is still in development");

            //var embed = new DiscordEmbedBuilder()
            //    .WithColor(DiscordColor.Azure)
            //    .WithAuthor(terms)
            //    .WithUrl(fullQueryLink)
            //    .WithImageUrl("http://i.imgur.com/G46fm8J.png")
            //    .WithTitle(CTX.User.ToString())
            //    .WithFooter(totalResults);
            //await CTX.RespondAsync(embed: embed);
        }

        [Hidden]
        [Command("image")]
        [Aliases("img")]
        [Description("Retrieve first image search result")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchImage(CommandContext CTX, [RemainingText] string terms = null)
        {
            await CTX.TriggerTypingAsync();
            await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} This command is still in development");

            //terms = WebUtility.UrlEncode(terms).Replace(" ", "+");
            //var res = await GetImageAsync(terms).ConfigureAwait(false);
            //var embed = new DiscordEmbedBuilder()
            //    .WithAuthor(terms)
            //    .WithTitle(CTX.User.ToString())
            //    .WithDescription(res.Link)
            //    .WithImageUrl("http://i.imgur.com/G46fm8J.png")
            //    .WithImageUrl(res.Link)
            //    .WithUrl($"https://www.google.rs/search?q={terms}&source=lnms&tbm=isch")
            //    .WithColor(DiscordColor.Red);
            //await CTX.RespondAsync(embed: embed);
        }

        [Command("shorten")]
        [Description("Shorten the inputted URL")]
        public async Task ShortenURL(CommandContext CTX, string URL)
        {
            await CTX.TriggerTypingAsync();
            var JSON = "";  // Load the configuration file
            using (var SRD = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)))
                JSON = await SRD.ReadToEndAsync();
            var shorten = new Google.Apis.Urlshortener.v1.Data.Url();
            UrlshortenerService service = new UrlshortenerService(new BaseClientService.Initializer()
            {
                ApiKey = JsonConvert.DeserializeObject<APITokenService.APITokenList>(JSON).GoogleToken,
                ApplicationName = "FlawBOT",
            });
            shorten.LongUrl = URL;
            await CTX.RespondAsync($"{service.Url.Insert(shorten).Execute().Id}");
        }

        [Hidden]
        [Command("revav")]
        [Description("Reverse image search someone's avatar")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchAvatarReverse(CommandContext CTX, [RemainingText] DiscordMember member)
        {
            await CTX.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("Google Reverse Image Search")
                .WithImageUrl(member.AvatarUrl)
                .WithUrl("https://images.google.com/searchbyimage?image_url={member.AvatarUrl}")
                .WithColor(DiscordColor.DarkButNotBlack);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("youtube")]
        [Aliases("yt")]
        [Description("Get the first YouTube video search result")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchVideoAsync(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Search query missing");
            else
            {
                var JSON = "";  // Load the configuration file
                using (var SRD = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)))
                    JSON = await SRD.ReadToEndAsync();
                string Token = JsonConvert.DeserializeObject<APITokenService.APITokenList>(JSON).GoogleToken;
                GoogleService.YoutubeService YTservice = new GoogleService.YoutubeService(Token);
                var output = await YTservice.GetFirstVideoResultAsync(query);
                await CTX.RespondAsync($"Search result for {Formatter.Bold(query)}\n {output}");
            }
        }

        [Command("ytchannel")]
        [Aliases("ytc")]
        [Description("Get the specified YouTube channel")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchChannelAsync(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Search query missing");
            else
            {
                var JSON = "";  // Load the configuration file
                using (var SRD = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)))
                    JSON = await SRD.ReadToEndAsync();
                string Token = JsonConvert.DeserializeObject<APITokenService.APITokenList>(JSON).GoogleToken;
                GoogleService.YoutubeService YTservice = new GoogleService.YoutubeService(Token);
                var output = await YTservice.GetEmbeddedResults(query, 5, "channel");
                await CTX.RespondAsync($"Search result for {Formatter.Bold(query)}", embed: output);
            }
        }

        [Command("ytlist")]
        [Aliases("ytl")]
        [Description("Get a list of YouTube search results")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchYoutube(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Search query missing");
            else
            {
                var JSON = "";  // Load the configuration file
                using (var SRD = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)))
                    JSON = await SRD.ReadToEndAsync();
                string Token = JsonConvert.DeserializeObject<APITokenService.APITokenList>(JSON).GoogleToken;
                GoogleService.YoutubeService YTservice = new GoogleService.YoutubeService(Token);
                var output = await YTservice.GetEmbeddedResults(query, 5, "video");
                await CTX.RespondAsync($"Search result for {Formatter.Bold(query)}", embed: output);
            }
        }

        [Command("ytplaylist")]
        [Aliases("ytp")]
        [Description("Get the specified YouTube playlist")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchPlaylistAsync(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Search query missing");
            else
            {
                var JSON = "";  // Load the configuration file
                using (var SRD = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)))
                    JSON = await SRD.ReadToEndAsync();
                string Token = JsonConvert.DeserializeObject<APITokenService.APITokenList>(JSON).GoogleToken;
                GoogleService.YoutubeService YTservice = new GoogleService.YoutubeService(Token);
                var output = await YTservice.GetEmbeddedResults(query, 5, "playlist");
                await CTX.RespondAsync($"Search result for {Formatter.Bold(query)}", embed: output);
            }
        }
    }
}