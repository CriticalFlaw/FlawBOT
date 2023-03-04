using DSharpPlus;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class YouTubeModule : ApplicationCommandModule
    {
        [SlashCommand("youtube_channel", "Retrieves a list of YouTube channels.")]
        public async Task YouTubeChannel(InteractionContext ctx, [Option("query", "Channels to find on YouTube.")] string query)
        {
            var output = await new YoutubeService().GetEmbeddedResults(query, 5, "channel").ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync("Search results for " + Formatter.Bold(query) + " on YouTube", output).ConfigureAwait(false);
        }

        [SlashCommand("youtube_playlist", "Retrieves a list of YouTube playlists.")]
        public async Task YouTubePlaylist(InteractionContext ctx, [Option("query", "Playlists to find on YouTube.")] string query)
        {
            var output = await new YoutubeService().GetEmbeddedResults(query, 5, "playlist").ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync("Search results for " + Formatter.Bold(query) + " on YouTube", output).ConfigureAwait(false);
        }

        [SlashCommand("youtube_video", "Retrieve a lists of YouTube videos.")]
        public async Task YouTubeSearch(InteractionContext ctx, [Option("query", "Videos to find on YouTube.")] string query)
        {
            var output = await new YoutubeService().GetEmbeddedResults(query, 5, "video").ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync("Search results for " + Formatter.Bold(query) + " on YouTube", output).ConfigureAwait(false);
        }

        [SlashCommand("youtube_search", "Retrieve the first YouTube search result.")]
        public async Task YouTubeVideo(InteractionContext ctx, [Option("query", "Video to find on YouTube.")] string query)
        {
            var output = await new YoutubeService().GetFirstVideoResultAsync(query).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}