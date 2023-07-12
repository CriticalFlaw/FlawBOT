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
        [SlashCommand("channel", "Returns a list of queried YouTube channels.")]
        public Task GetYouTubeChannels(InteractionContext ctx, [Option("query", "Channels to find on YouTube.")] string query)
        {
            return GetYouTubePost(ctx, query, YouTubeSearch.Channel);
        }

        [SlashCommand("playlist", "Returns a list of queried YouTube playlists.")]
        public Task GetYouTubePlaylists(InteractionContext ctx, [Option("query", "Playlists to find on YouTube.")] string query)
        {
            return GetYouTubePost(ctx, query, YouTubeSearch.Playlist);
        }

        [SlashCommand("video", "Returns a list of queried YouTube videos.")]
        public Task GetYouTubeVideos(InteractionContext ctx, [Option("query", "Videos to find on YouTube.")] string query)
        {
            return GetYouTubePost(ctx, query, YouTubeSearch.Channel);
        }

        [SlashCommand("search", "Returns the first YouTube search result.")]
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