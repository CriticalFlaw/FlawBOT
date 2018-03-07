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
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task ShortenURL(CommandContext ctx, [RemainingText] string query)
        {
            if (Uri.IsWellFormedUriString(query, UriKind.RelativeOrAbsolute)) // && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                await ctx.TriggerTypingAsync();
                var shortenService = new ShortenService();
                var output = shortenService.shortenUrl(query);
                await ctx.RespondAsync(output);
            }
            else
                await ctx.RespondAsync(":warning: A valid URL is required! :warning:");
        }

        [Command("revav")]
        [Description("Reverse Googe Image Search user avatar")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchAvatarReverse(CommandContext ctx, [RemainingText] DiscordMember member)
        {
            if (member == null)
                member = ctx.Member;
            await ctx.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("Google Reverse Image Search Results")
                .WithImageUrl(member.AvatarUrl)
                .WithUrl($"https://images.google.com/searchbyimage?image_url={member.AvatarUrl}")
                .WithColor(DiscordColor.Purple);
            await ctx.RespondAsync(embed: output.Build());
        }

        [Command("youtube")]
        [Aliases("yt")]
        [Description("Get the first YouTube video search result")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchVideoAsync(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await ctx.RespondAsync("https://www.youtube.com/watch?v=rFA_auWj0rQ");
            else
            {
                await ctx.TriggerTypingAsync();
                var youTubeService = new YoutubeService();
                var output = await youTubeService.GetFirstVideoResultAsync(query);
                await ctx.RespondAsync(output);
            }
        }

        [Command("ytchannel")]
        [Aliases("ytc")]
        [Description("Get the specified YouTube channel")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchChannelAsync(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await ctx.RespondAsync(":warning: YouTube channel search query is required! :warning:");
            else
            {
                await ctx.TriggerTypingAsync();
                var youTubeService = new YoutubeService();
                var output = await youTubeService.GetEmbeddedResults(query, 5, "channel");
                await ctx.RespondAsync($"Search results for {Formatter.Bold(query)}", embed: output);
            }
        }

        [Command("ytvideos")]
        [Aliases("ytv")]
        [Description("Get a list of YouTube search results")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchYoutube(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await ctx.RespondAsync(":warning: YouTube video search query is required! :warning:");
            else
            {
                await ctx.TriggerTypingAsync();
                var youTubeService = new YoutubeService();
                var output = await youTubeService.GetEmbeddedResults(query, 5, "video");
                await ctx.RespondAsync($"Search results for {Formatter.Bold(query)}", embed: output);
            }
        }

        [Command("ytplaylist")]
        [Aliases("ytp")]
        [Description("Get the specified YouTube playlist")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchPlaylistAsync(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await ctx.RespondAsync(":warning: YouTube playlist search query is required! :warning:");
            else
            {
                await ctx.TriggerTypingAsync();
                var youTubeService = new YoutubeService();
                var output = await youTubeService.GetEmbeddedResults(query, 5, "playlist");
                await ctx.RespondAsync($"Search results for {Formatter.Bold(query)}", embed: output);
            }
        }
    }
}