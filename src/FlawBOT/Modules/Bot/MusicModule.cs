using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;
using FlawBOT.Common;
using FlawBOT.Models;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT.Modules
{
    public class MusicModule : BaseCommandModule
    {
        public MusicModule(MusicService service, YoutubeService youTube)
        {
            Service = service;
            YouTube = youTube;
        }

        private MusicService Service { get; }
        private MusicPlayer Player { get; set; }
        private YoutubeService YouTube { get; }

        #region COMMAND_STOP

        [Command("stop")]
        [Description("Stop audio playback and leave the voice channel.")]
        public async Task StopSong(CommandContext ctx)
        {
            await Player.StopAsync();
            await Player.DestroyPlayerAsync();
            await ctx.RespondAsync(":stop_button: Stopping Playback...").ConfigureAwait(false);
        }

        #endregion COMMAND_STOP

        #region COMMAND_PAUSE

        [Command("pause")]
        [Description("Pause audio playback.")]
        public async Task PauseSong(CommandContext ctx)
        {
            await Player.PauseAsync();
            await ctx.RespondAsync(":pause_button: Pausing Playback...").ConfigureAwait(false);
        }

        #endregion COMMAND_PAUSE

        #region COMMAND_RESUME

        [Command("resume"), Aliases("unpause")]
        [Description("Resume audio playback.")]
        public async Task ResumeAsync(CommandContext ctx)
        {
            await Player.ResumeAsync();
            await ctx.RespondAsync(":play_pause: Resuming Playback...").ConfigureAwait(false);
        }

        #endregion COMMAND_RESUME

        #region COMMAND_VOLUME

        [Command("volume"), Aliases("v")]
        [Description("Set audio playback volume.")]
        public async Task SetVolume(CommandContext ctx,
            [Description("Audio volume. Can be set to 0-150 (default 100).")]
            int volume = 100)
        {
            if (volume < 0 || volume > 150)
            {
                await ctx.RespondAsync(":warning: Volume must be greater than 0, and less than or equal to 150.")
                    .ConfigureAwait(false);
                return;
            }

            await Player.SetVolumeAsync(volume);
            await ctx.RespondAsync($":speaker: Volume set to {volume}%.").ConfigureAwait(false);
        }

        #endregion COMMAND_VOLUME

        #region COMMAND_RESTART

        [Command("restart"), Aliases("replay")]
        [Description("Restarts the playback of the current track.")]
        public async Task RestartSong(CommandContext ctx)
        {
            var track = Player.NowPlaying;
            await Player.RestartAsync();
            await ctx.RespondAsync(
                    $":play_pause: Restarting {Formatter.Bold(Formatter.Sanitize(track.Track.Title))} by {Formatter.Bold(Formatter.Sanitize(track.Track.Author))}...")
                .ConfigureAwait(false);
        }

        #endregion COMMAND_RESTART

        #region COMMAND_NOWPLAYING

        [Command("nowplaying"), Aliases("np")]
        [Description("Displays information about currently-played track.")]
        public async Task NowPlaying(CommandContext ctx)
        {
            var track = Player.NowPlaying;
            if (Player.NowPlaying.Track?.TrackString == null)
                await ctx.RespondAsync("Currently not playing anything...").ConfigureAwait(false);
            else
                await ctx.RespondAsync(
                        $":musical_note: Now playing: {Formatter.Bold(Formatter.Sanitize(track.Track.Title))} by {Formatter.Bold(Formatter.Sanitize(track.Track.Author))} [{MusicService.ToDurationString(Player.GetCurrentPosition())}/{MusicService.ToDurationString(Player.NowPlaying.Track.Length)}] requested by {Formatter.Bold(Formatter.Sanitize(Player.NowPlaying.Requester.DisplayName))}.")
                    .ConfigureAwait(false);
        }

        #endregion COMMAND_NOWPLAYING

        #region OVERLOAD_VALIDATION

        public override async Task BeforeExecutionAsync(CommandContext ctx)
        {
            // Check that the user is in a voice channel
            var channel = ctx.Member.VoiceState?.Channel;
            if (channel == null)
            {
                await ctx.RespondAsync("You need to be in a voice channel.").ConfigureAwait(false);
                return;
            }

            // Check that the user in the same voice channel
            var userState = ctx.Guild.CurrentMember?.VoiceState?.Channel;
            if (userState != null && channel != userState)
            {
                await ctx.RespondAsync("You need to be in the same voice channel.").ConfigureAwait(false);
                return;
            }

            // Connect the music play to the voice channel
            Player = await Service.GetOrCreateDataAsync(ctx.Guild);
            Player.CommandChannel = ctx.Channel;
            await base.BeforeExecutionAsync(ctx);
        }

        #endregion OVERLOAD_VALIDATION

        #region COMMAND_PLAY

        [Priority(1)]
        [Command("play"), Aliases("p")]
        [Description("Play audio from provided URL or search by specified query.")]
        public async Task PlaySong(CommandContext ctx,
            [Description("URL from which to play audio")]
            Uri uri)
        {
            var trackLoad = await Service.GetTracksAsync(uri);
            await Play(ctx, trackLoad);
        }

        [Priority(0)]
        [Command("play")]
        public async Task PlaySong(CommandContext ctx, [RemainingText] string query)
        {
            var results = await YouTube.GetMusicDataAsync(query);
            if (!results.Any())
            {
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_COMMON, EmbedType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            var trackLoad = await Service.GetTracksAsync(new Uri($"https://youtu.be/{results.FirstOrDefault().Id}"));
            await Play(ctx, trackLoad);
        }

        public async Task Play(CommandContext ctx, LavalinkLoadResult results)
        {
            var audio = results.Tracks.FirstOrDefault();
            if (results.LoadResultType == LavalinkLoadResultType.LoadFailed || audio is null)
            {
                await ctx.RespondAsync(":mag: No tracks were found at specified link.").ConfigureAwait(false);
                return;
            }

            Player.Enqueue(new MusicData(audio, ctx.Member));
            await Player.CreatePlayerAsync(ctx.Member.VoiceState.Channel);
            await Player.PlayAsync();

            await ctx.RespondAsync(
                    $":play_pause: Now playing: {Formatter.Bold(Formatter.Sanitize(audio.Title))} by {Formatter.Bold(Formatter.Sanitize(audio.Author))}.")
                .ConfigureAwait(false);
        }

        #endregion COMMAND_PLAY
    }
}