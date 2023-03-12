using DSharpPlus;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;
using static FlawBOT.Services.YoutubeService;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("youtube", "Slash command group for YouTube commands.")]
    public class YouTubeModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns a list of channels found on YouTube.
        /// </summary>
        [SlashCommand("channel", "Retrieves a list of YouTube channels.")]
        public Task GetYouTubeChannels(InteractionContext ctx, [Option("query", "Channels to find on YouTube.")] string query)
        {
            return GetYouTubePost(ctx, query, YouTubeSearch.Channel);
        }

        /// <summary>
        /// Returns a list of playlists found on YouTube.
        /// </summary>
        [SlashCommand("playlist", "Retrieves a list of YouTube playlists.")]
        public Task GetYouTubePlaylists(InteractionContext ctx, [Option("query", "Playlists to find on YouTube.")] string query)
        {
            return GetYouTubePost(ctx, query, YouTubeSearch.Playlist);
        }

        /// <summary>
        /// Returns a list of videos found on YouTube.
        /// </summary>
        [SlashCommand("video", "Retrieve a lists of YouTube videos.")]
        public Task GetYouTubeVideos(InteractionContext ctx, [Option("query", "Videos to find on YouTube.")] string query)
        {
            return GetYouTubePost(ctx, query, YouTubeSearch.Channel);
        }

        /// <summary>
        /// Returns first video found on YouTube.
        /// </summary>
        [SlashCommand("search", "Retrieve the first YouTube search result.")]
        public async Task GetYouTubeSearch(InteractionContext ctx, [Option("query", "Video to find on YouTube.")] string query)
        {
            var output = await new YoutubeService().GetFirstVideoResultAsync(query).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        private static async Task GetYouTubePost(InteractionContext ctx, string query, YouTubeSearch type)
        {
            var output = await new YoutubeService().GetEmbeddedResults(query, 5, type).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync("Search results for " + Formatter.Bold(query) + " on YouTube", output).ConfigureAwait(false);
        }
    }
}