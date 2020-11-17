using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Group("youtube")]
    [Aliases("yt")]
    [Description("Commands for finding YouTube videos, channels and playlists")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class YouTubeModule : BaseCommandModule
    {
        #region COMMAND_CHANNEL

        [Command("channel"), Aliases("channels", "chn")]
        [Description("Retrieve a list of YouTube channel given a query")]
        public async Task YtChannel(CommandContext ctx,
            [Description("Channels to find on YouTube"), RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await new YoutubeService().GetEmbeddedResults(query, 5, "channel").ConfigureAwait(false);
            await ctx.RespondAsync("Search results for " + Formatter.Bold(query) + " on YouTube", embed: results)
                .ConfigureAwait(false);
        }

        #endregion COMMAND_CHANNEL

        #region COMMAND_PLAYLIST

        [Command("playlist"), Aliases("playlists", "list")]
        [Description("Retrieve a list of YouTube playlists given a query")]
        public async Task YtPlaylist(CommandContext ctx,
            [Description("Playlist to find on YouTube"), RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await new YoutubeService().GetEmbeddedResults(query, 5, "playlist").ConfigureAwait(false);
            await ctx.RespondAsync("Search results for " + Formatter.Bold(query) + " on YouTube", embed: results)
                .ConfigureAwait(false);
        }

        #endregion COMMAND_PLAYLIST

        #region COMMAND_SEARCH

        [Command("search"), Aliases("find", "watch")]
        [Description("Retrieve the first YouTube search result given a query")]
        public async Task YtVideo(CommandContext ctx,
            [Description("First result video to find on YouTube"), RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await new YoutubeService().GetFirstVideoResultAsync(query).ConfigureAwait(false);
            await ctx.RespondAsync(results).ConfigureAwait(false);
        }

        #endregion COMMAND_SEARCH

        #region COMMAND_VIDEO

        [Command("video"), Aliases("videos", "vid")]
        [Description("Retrieve a list of YouTube videos given a query")]
        public async Task YtSearch(CommandContext ctx,
            [Description("Video to find on YouTube"), RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await new YoutubeService().GetEmbeddedResults(query, 5, "video").ConfigureAwait(false);
            await ctx.RespondAsync("Search results for " + Formatter.Bold(query) + " on YouTube", embed: results)
                .ConfigureAwait(false);
        }

        #endregion COMMAND_VIDEO
    }
}