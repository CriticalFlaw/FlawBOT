using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using FlawBOT.Models;
using FlawBOT.Services;
using FlawBOT.Services.Search;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
{
    [Group("youtube")]
    [Aliases("yt")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class YouTubeModule : BaseCommandModule
    {
        #region COMMAND_VIDEO

        [Command("video")]
        [Aliases("vid")]
        [Description("Get the first YouTube video search result")]
        public async Task YouTubeVideo(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await ctx.RespondAsync("https://www.youtube.com/watch?v=rFA_auWj0rQ");
            else
            {
                var service = new YoutubeService();
                var output = await service.GetFirstVideoResultAsync(query);
                await ctx.RespondAsync(output);
            }
        }

        #endregion COMMAND_VIDEO

        #region COMMAND_CHANNEL

        [Command("channel")]
        [Aliases("chn")]
        [Description("Get the specified YouTube channel")]
        public async Task YouTubeChannel(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await BotServices.SendEmbedAsync(ctx, ":warning: Channel search query is required!", EmbedType.Warning);
            else
            {
                var service = new YoutubeService();
                var output = await service.GetEmbeddedResults(query, 5, "channel");
                await ctx.RespondAsync($"Search results for {Formatter.Bold(query)}", embed: output);
            }
        }

        #endregion COMMAND_CHANNEL

        #region COMMAND_SEARCH

        [Command("search")]
        [Aliases("find")]
        [Description("Get a list of YouTube search results")]
        public async Task YouTubeSearch(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await BotServices.SendEmbedAsync(ctx, ":warning: Video search query is required!", EmbedType.Warning);
            else
            {
                var service = new YoutubeService();
                var output = await service.GetEmbeddedResults(query, 5, "video");
                await ctx.RespondAsync($"Search results for {Formatter.Bold(query)}", embed: output);
            }
        }

        #endregion COMMAND_SEARCH

        #region COMMAND_PLAYLIST

        [Command("playlist")]
        [Aliases("list")]
        [Description("Get the specified YouTube playlist")]
        public async Task YouTubePlaylist(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await BotServices.SendEmbedAsync(ctx, ":warning: Playlist search query is required!", EmbedType.Warning);
            else
            {
                var service = new YoutubeService();
                var output = await service.GetEmbeddedResults(query, 5, "playlist");
                await ctx.RespondAsync($"Search results for {Formatter.Bold(query)}", embed: output);
            }
        }

        #endregion COMMAND_PLAYLIST
    }
}