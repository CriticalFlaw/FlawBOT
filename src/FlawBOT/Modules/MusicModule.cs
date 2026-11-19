using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using FlawBOT.Common;
using FlawBOT.Services;
using Lavalink4NET.Players;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("music", "Slash command group for music commands.")]
    [SlashRequireGuild]
    public class MusicModule(MusicService service) : ApplicationCommandModule
    {
        private MusicService Service { get; } = service;

        [SlashCommand("info", "Returns the current audio track information.")]
        public async Task NowPlaying(InteractionContext ctx)
        {
            var player = await GetPlayerAsync(ctx).ConfigureAwait(false);
            if (player is null) return;

            var track = player.CurrentTrack;
            if (track is null)
            {
                await ctx.CreateResponseAsync("Currently not playing anything...").ConfigureAwait(false);
                return;
            }

            var position = player.Position?.Position ?? System.TimeSpan.Zero;
            var requester = Service.GetRequester(ctx.Guild.Id);
            var requestedBy = requester is null ? string.Empty : $" requested by {Formatter.Bold(Formatter.Sanitize(requester.DisplayName))}";
            await ctx.CreateResponseAsync($":musical_note: Now playing: {MusicService.DescribeTrack(track)} [{MusicService.ToDurationString(position)}/{MusicService.ToDurationString(track.Duration)}]{requestedBy}.").ConfigureAwait(false);
        }

        [SlashCommand("pause", "Pauses audio playback.")]
        public async Task PauseSong(InteractionContext ctx)
        {
            var player = await GetPlayerAsync(ctx).ConfigureAwait(false);
            if (player is null) return;
            await player.PauseAsync().ConfigureAwait(false);
            await ctx.CreateResponseAsync(":pause_button: Pausing Playback...").ConfigureAwait(false);
        }

        [SlashCommand("play", "Plays audio from a URL or search query.")]
        public async Task PlaySong(InteractionContext ctx, [Option("query", "URL or search query to play.")] string query)
        {
            // Loading tracks can take longer than the three seconds Discord allows for a response.
            await ctx.DeferAsync().ConfigureAwait(false);

            var (player, error) = await Service.GetPlayerAsync(ctx, true).ConfigureAwait(false);
            if (player is null)
            {
                await BotServices.SendDeferredAsync(ctx, error, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var track = await Service.LoadTrackAsync(query).ConfigureAwait(false);
            if (track is null)
            {
                await BotServices.SendDeferredAsync(ctx, "No tracks were found for that link or search.", ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            Service.SetRequester(ctx.Guild.Id, ctx.Member);
            await player.PlayAsync(track).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, $":play_pause: Now playing: {MusicService.DescribeTrack(track)}.").ConfigureAwait(false);
        }

        [SlashCommand("restart", "Restarts audio playback.")]
        public async Task RestartSong(InteractionContext ctx)
        {
            var player = await GetPlayerAsync(ctx).ConfigureAwait(false);
            if (player is null) return;

            var track = player.CurrentTrack;
            if (track is null)
            {
                await ctx.CreateResponseAsync("Currently not playing anything...").ConfigureAwait(false);
                return;
            }

            await player.SeekAsync(System.TimeSpan.Zero).ConfigureAwait(false);
            await ctx.CreateResponseAsync($":play_pause: Restarting {MusicService.DescribeTrack(track)}...").ConfigureAwait(false);
        }

        [SlashCommand("resume", "Resumes audio playback.")]
        public async Task ResumeAsync(InteractionContext ctx)
        {
            var player = await GetPlayerAsync(ctx).ConfigureAwait(false);
            if (player is null) return;
            await player.ResumeAsync().ConfigureAwait(false);
            await ctx.CreateResponseAsync(":play_pause: Resuming Playback...").ConfigureAwait(false);
        }

        [SlashCommand("stop", "Stops audio playback and leave the voice channel.")]
        public async Task StopSong(InteractionContext ctx)
        {
            var player = await GetPlayerAsync(ctx).ConfigureAwait(false);
            if (player is null) return;
            await player.StopAsync().ConfigureAwait(false);
            await player.DisconnectAsync().ConfigureAwait(false);
            await ctx.CreateResponseAsync(":stop_button: Stopping Playback...").ConfigureAwait(false);
        }

        [SlashCommand("volume", "Changes audio playback volume between 0% and 100%.")]
        public async Task SetVolume(InteractionContext ctx, [Option("volume", "Volume of the playback.")][Minimum(0)][Maximum(100)] long volume = 100)
        {
            var player = await GetPlayerAsync(ctx).ConfigureAwait(false);
            if (player is null) return;
            await player.SetVolumeAsync(volume / 100f).ConfigureAwait(false);
            await ctx.CreateResponseAsync($":speaker: Volume set to {volume}%.").ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets the existing player for the guild, or responds with the reason it is unavailable and returns null.
        /// </summary>
        private async Task<LavalinkPlayer> GetPlayerAsync(InteractionContext ctx)
        {
            var (player, error) = await Service.GetPlayerAsync(ctx, false).ConfigureAwait(false);
            if (player is null)
                await BotServices.SendResponseAsync(ctx, error, ResponseType.Warning).ConfigureAwait(false);
            return player;
        }
    }
}
