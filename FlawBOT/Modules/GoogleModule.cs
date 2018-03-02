using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using FlawBOT.Services;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class GoogleModule
    {
        [Command("shorten")]
        [Description("Shorten the inputted URL")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task ShortenURL(CommandContext CTX, [RemainingText] string query)
        {
            if (Uri.TryCreate(query, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                await CTX.TriggerTypingAsync();
                ShortenService shortenService = new ShortenService();
                var output = shortenService.ShortenURL(query);
                await CTX.RespondAsync(output);
            }
            else
                await CTX.RespondAsync(":warning: Please provide a valid URL to shorten, include **http://** or **https://**");
        }

        [Command("youtube")]
        [Aliases("yt")]
        [Description("Get the first YouTube video search result")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchVideoAsync(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync("https://www.youtube.com/watch?v=rFA_auWj0rQ");
            else
            {
                await CTX.TriggerTypingAsync();
                YoutubeService YTservice = new YoutubeService();
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
                await CTX.RespondAsync(":warning: Please provide a channel to search for...");
            else
            {
                await CTX.TriggerTypingAsync();
                YoutubeService YTservice = new YoutubeService();
                var output = await YTservice.GetEmbeddedResults(query, 5, "channel");
                await CTX.RespondAsync($"Search results for {Formatter.Bold(query)}", embed: output);
            }
        }

        [Command("ytvideos")]
        [Aliases("ytv")]
        [Description("Get a list of YouTube search results")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchYoutube(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync(":warning: Please provide a video list to search for...");
            else
            {
                await CTX.TriggerTypingAsync();
                YoutubeService YTservice = new YoutubeService();
                var output = await YTservice.GetEmbeddedResults(query, 5, "video");
                await CTX.RespondAsync($"Search results for {Formatter.Bold(query)}", embed: output);
            }
        }

        [Command("ytplaylist")]
        [Aliases("ytp")]
        [Description("Get the specified YouTube playlist")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchPlaylistAsync(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync(":warning: Please provide a playlist to search for...");
            else
            {
                await CTX.TriggerTypingAsync();
                YoutubeService YTservice = new YoutubeService();
                var output = await YTservice.GetEmbeddedResults(query, 5, "playlist");
                await CTX.RespondAsync($"Search results for {Formatter.Bold(query)}", embed: output);
            }
        }
    }
}