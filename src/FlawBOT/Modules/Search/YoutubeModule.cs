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
    [Description("Commands for finding YouTube videos, channels and playlists")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class YouTubeModule : BaseCommandModule
    {
        #region COMMAND_CHANNEL

        [Command("channel")]
        [Aliases("channels", "chn")]
        [Description("Retrieve a list of YouTube channel given a query")]
        public async Task YouTubeChannel(CommandContext ctx,
            [Description("Channels to find on YouTube")] [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await BotServices.SendEmbedAsync(ctx, "Channel search query is required!", EmbedType.Warning);
            else
            {
                var service = new YoutubeService();
                var output = await service.GetEmbeddedResults(query, 5, "channel");
                await ctx.RespondAsync("Search results for " + Formatter.Bold(query), embed: output);
            }
        }

        #endregion COMMAND_CHANNEL

        #region COMMAND_PLAYLIST

        [Command("playlist")]
        [Aliases("playlists", "list")]
        [Description("Retrieve a list of YouTube playlists given a query")]
        public async Task YouTubePlaylist(CommandContext ctx,
            [Description("Playlist to find on YouTube")] [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await BotServices.SendEmbedAsync(ctx, "Playlist search query is required!", EmbedType.Warning);
            else
            {
                var service = new YoutubeService();
                var output = await service.GetEmbeddedResults(query, 5, "playlist");
                await ctx.RespondAsync("Search results for " + Formatter.Bold(query), embed: output);
            }
        }

        #endregion COMMAND_PLAYLIST

        #region COMMAND_SEARCH

        [Command("search")]
        [Aliases("find")]
        [Description("Retrieve the first YouTube search result given a query")]
        public async Task YouTubeVideo(CommandContext ctx,
            [Description("First result video to find on YouTube")] [RemainingText] string query)
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

        #endregion COMMAND_SEARCH

        #region COMMAND_VIDEO

        [Command("video")]
        [Aliases("videos", "vid")]
        [Description("Retrieve a list of YouTube videos given a query")]
        public async Task YouTubeSearch(CommandContext ctx,
            [Description("Video to find on YouTube")] [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await BotServices.SendEmbedAsync(ctx, "Video search query is required!", EmbedType.Warning);
            else
            {
                var service = new YoutubeService();
                var output = await service.GetEmbeddedResults(query, 5, "video");
                await ctx.RespondAsync("Search results for " + Formatter.Bold(query), embed: output);
            }
        }

        #endregion COMMAND_VIDEO
    }
}