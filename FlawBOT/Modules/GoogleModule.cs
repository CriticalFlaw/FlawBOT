using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
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
            if (Uri.IsWellFormedUriString(query, UriKind.RelativeOrAbsolute))  // && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                await CTX.TriggerTypingAsync();
                ShortenService shortenService = new ShortenService();
                var output = shortenService.ShortenURL(query);
                await CTX.RespondAsync(output);
            }
            else
                await CTX.RespondAsync(":warning: A valid URL is required! :warning:");
        }

        [Command("revav")]
        [Description("Reverse Googe Image Search user avatar")]
        [Cooldown(3, 5, CooldownBucketType.User)]
        public async Task SearchAvatarReverse(CommandContext CTX, [RemainingText] DiscordMember member)
        {
            if (member == null)
                member = CTX.Member;
            await CTX.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("Google Reverse Image Search Results")
                .WithImageUrl(member.AvatarUrl)
                .WithUrl($"https://images.google.com/searchbyimage?image_url={member.AvatarUrl}")
                .WithColor(DiscordColor.Purple);
            await CTX.RespondAsync(embed: output.Build());
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
                await CTX.RespondAsync(":warning: YouTube channel search query is required! :warning:");
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
                await CTX.RespondAsync(":warning: YouTube video search query is required! :warning:");
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
                await CTX.RespondAsync(":warning: YouTube playlist search query is required! :warning:");
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