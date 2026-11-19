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
    public class YouTubeModule(YoutubeService service) : ApplicationCommandModule
    {
        private YoutubeService Service { get; } = service;

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

        [SlashCommand("search", "Returns the first YouTube search result.")]
        public async Task GetYouTubeSearch(InteractionContext ctx, [Option("query", "Video to find on YouTube.")] string query)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await Service.GetFirstVideoResultAsync(query).ConfigureAwait(false);
            if (output is null)
                await BotServices.SendDeferredAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
            else
                await BotServices.SendDeferredAsync(ctx, output).ConfigureAwait(false);
        }

        [SlashCommand("video", "Returns a list of queried YouTube videos.")]
        public Task GetYouTubeVideos(InteractionContext ctx, [Option("query", "Videos to find on YouTube.")] string query)
        {
            return GetYouTubePost(ctx, query, YouTubeSearch.Video);
        }

        private async Task GetYouTubePost(InteractionContext ctx, string query, YouTubeSearch type)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await Service.GetEmbeddedResults(query, 5, type).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON, content: "Search results for " + Formatter.Bold(Formatter.Sanitize(query)) + " on YouTube").ConfigureAwait(false);
        }
    }
}
