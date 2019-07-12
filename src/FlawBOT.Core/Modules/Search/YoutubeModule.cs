using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using FlawBOT.Framework.Services;
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
            if (!BotServices.CheckUserInput(query)) return;
            var service = new YoutubeService();
            var results = await service.GetEmbeddedResults(query, 5, "channel");
            await ctx.RespondAsync("Search results for " + Formatter.Bold(query), embed: results);
        }

        #endregion COMMAND_CHANNEL

        #region COMMAND_PLAYLIST

        [Command("playlist")]
        [Aliases("playlists", "list")]
        [Description("Retrieve a list of YouTube playlists given a query")]
        public async Task YouTubePlaylist(CommandContext ctx,
            [Description("Playlist to find on YouTube")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var service = new YoutubeService();
            var results = await service.GetEmbeddedResults(query, 5, "playlist");
            await ctx.RespondAsync("Search results for " + Formatter.Bold(query), embed: results);
        }

        #endregion COMMAND_PLAYLIST

        #region COMMAND_SEARCH

        [Command("search")]
        [Aliases("find")]
        [Description("Retrieve the first YouTube search result given a query")]
        public async Task YouTubeVideo(CommandContext ctx,
            [Description("First result video to find on YouTube")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var service = new YoutubeService();
            var results = await service.GetFirstVideoResultAsync(query);
            await ctx.RespondAsync(results);
        }

        #endregion COMMAND_SEARCH

        #region COMMAND_VIDEO

        [Command("video")]
        [Aliases("videos", "vid")]
        [Description("Retrieve a list of YouTube videos given a query")]
        public async Task YouTubeSearch(CommandContext ctx,
            [Description("Video to find on YouTube")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var service = new YoutubeService();
            var results = await service.GetEmbeddedResults(query, 5, "video");
            await ctx.RespondAsync("Search results for " + Formatter.Bold(query), embed: results);
        }

        #endregion COMMAND_VIDEO
    }
}