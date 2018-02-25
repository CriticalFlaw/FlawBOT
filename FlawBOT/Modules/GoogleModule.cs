using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Services;
using Google.Apis.Services;
using Google.Apis.Urlshortener.v1;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class GoogleModule
    {
        [Command("shorten")]
        [Description("Shorten the inputted URL")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task ShortenURL(CommandContext CTX, string URL)
        {
            await CTX.TriggerTypingAsync();
            var shorten = new Google.Apis.Urlshortener.v1.Data.Url();
            APITokenService service = new APITokenService();
            UrlshortenerService google = new UrlshortenerService(new BaseClientService.Initializer()
            {
                ApiKey = service.GetAPIToken("google"),
                ApplicationName = "FlawBOT",
            });
            shorten.LongUrl = URL;
            await CTX.RespondAsync($"{google.Url.Insert(shorten).Execute().Id}");
        }

        [Command("youtube")]
        [Aliases("yt")]
        [Description("Get the first YouTube video search result")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchVideoAsync(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a video to search for...");
            else
            {
                APITokenService service = new APITokenService();
                string Token = service.GetAPIToken("google");
                GoogleService.YoutubeService YTservice = new GoogleService.YoutubeService(Token);
                var output = await YTservice.GetFirstVideoResultAsync(query);
                await CTX.RespondAsync(output);
            }
        }

        [Command("ytchannel")]
        [Aliases("ytc")]
        [Description("Get the specified YouTube channel")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchChannelAsync(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a channel to search for...");
            else
            {
                APITokenService service = new APITokenService();
                string Token = service.GetAPIToken("google");
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
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a video list to search for...");
            else
            {
                APITokenService service = new APITokenService();
                string Token = service.GetAPIToken("google");
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
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide a playlist to search for...");
            else
            {
                APITokenService service = new APITokenService();
                string Token = service.GetAPIToken("google");
                GoogleService.YoutubeService YTservice = new GoogleService.YoutubeService(Token);
                var output = await YTservice.GetEmbeddedResults(query, 5, "playlist");
                await CTX.RespondAsync($"Search result for {Formatter.Bold(query)}", embed: output);
            }
        }
    }
}