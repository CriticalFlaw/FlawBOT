using DSharpPlus;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class YouTubeModule : ApplicationCommandModule
    {
        #region COMMAND_CHANNEL

        [SlashCommand("channel", "Retrieve a list of YouTube channels.")]
        public async Task YtChannel(InteractionContext ctx, [Option("query", "Channels to find on YouTube.")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await new YoutubeService().GetEmbeddedResults(query, 5, "channel").ConfigureAwait(false);
            await ctx.CreateResponseAsync("Search results for " + Formatter.Bold(query) + " on YouTube", results).ConfigureAwait(false);
        }

        #endregion COMMAND_CHANNEL

        #region COMMAND_PLAYLIST

        [SlashCommand("playlist", "Retrieve a list of YouTube playlists.")]
        public async Task YtPlaylist(InteractionContext ctx, [Option("query", "Playlists to find on YouTube.")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await new YoutubeService().GetEmbeddedResults(query, 5, "playlist").ConfigureAwait(false);
            await ctx.CreateResponseAsync("Search results for " + Formatter.Bold(query) + " on YouTube", results).ConfigureAwait(false);
        }

        #endregion COMMAND_PLAYLIST

        #region COMMAND_SEARCH

        [SlashCommand("search", "Retrieve the first YouTube search result.")]
        public async Task YtVideo(InteractionContext ctx, [Option("query", "First result video to find on YouTube.")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await new YoutubeService().GetFirstVideoResultAsync(query).ConfigureAwait(false);
            await ctx.CreateResponseAsync(results).ConfigureAwait(false);
        }

        #endregion COMMAND_SEARCH

        #region COMMAND_VIDEO

        [SlashCommand("video", "Retrieve a list of YouTube videos.")]
        public async Task YtSearch(InteractionContext ctx, [Option("query", "Videos to find on YouTube.")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await new YoutubeService().GetEmbeddedResults(query, 5, "video").ConfigureAwait(false);
            await ctx.CreateResponseAsync("Search results for " + Formatter.Bold(query) + " on YouTube", results).ConfigureAwait(false);
        }

        #endregion COMMAND_VIDEO
    }
}