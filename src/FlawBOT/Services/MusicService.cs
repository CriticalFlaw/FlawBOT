using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using Lavalink4NET;
using Lavalink4NET.Clients;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    /// <summary>
    ///     Wraps the Lavalink4NET audio service and keeps track of which text channel each guild's music commands came from.
    /// </summary>
    public sealed class MusicService
    {
        private static readonly EventId LogEvent = new(1001, "Music");
        private readonly ConcurrentDictionary<ulong, DiscordChannel> _commandChannels = new();
        private readonly ConcurrentDictionary<ulong, DiscordMember> _requesters = new();
        private readonly ILogger<MusicService> _logger;

        public MusicService(IAudioService audio, ILogger<MusicService> logger)
        {
            Audio = audio;
            _logger = logger;
            Audio.TrackException += OnTrackExceptionAsync;
        }

        public IAudioService Audio { get; }

        /// <summary>
        ///     Finds the guild's player, optionally joining the member's voice channel.
        ///     Returns an error message instead of a player when the request can't be satisfied.
        /// </summary>
        public async ValueTask<(LavalinkPlayer Player, string Error)> GetPlayerAsync(InteractionContext ctx, bool connectToVoiceChannel)
        {
            var channelBehavior = connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None;
            var retrieveOptions = new PlayerRetrieveOptions(channelBehavior, MemberVoiceStateBehavior.RequireSame);
            var result = await Audio.Players.RetrieveAsync(ctx.Guild.Id, ctx.Member?.VoiceState?.Channel?.Id, PlayerFactory.Default,
                Options.Create(new LavalinkPlayerOptions()), retrieveOptions).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                _commandChannels[ctx.Guild.Id] = ctx.Channel;
                return (result.Player, null);
            }

            return (null, result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You need to be in a voice channel.",
                PlayerRetrieveStatus.VoiceChannelMismatch => "You need to be in the same voice channel.",
                PlayerRetrieveStatus.BotNotConnected => "Currently not playing anything...",
                _ => "Unable to connect to the voice channel."
            });
        }

        /// <summary>
        ///     Loads a track from a URL, or searches YouTube when given plain text.
        /// </summary>
        public ValueTask<LavalinkTrack> LoadTrackAsync(string query)
        {
            query = query?.Trim();
            var mode = BotServices.IsWebUrl(query, out _) ? TrackSearchMode.None : TrackSearchMode.YouTube;
            return Audio.Tracks.LoadTrackAsync(query, mode);
        }

        public void SetRequester(ulong guildId, DiscordMember member)
        {
            _requesters[guildId] = member;
        }

        public DiscordMember GetRequester(ulong guildId)
        {
            return _requesters.TryGetValue(guildId, out var member) ? member : null;
        }

        /// <summary>
        ///     Pauses playback when everyone except bots has left the player's voice channel.
        /// </summary>
        public async Task HandleVoiceStateUpdatedAsync(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            if (e.Guild is null || e.User.Id == sender.CurrentUser.Id) return;
            if (!Audio.Players.TryGetPlayer(e.Guild.Id, out LavalinkPlayer player)) return;

            var channel = e.Guild.GetChannel(player.VoiceChannelId);
            if (channel is null || e.Before?.Channel?.Id != channel.Id || e.After?.Channel?.Id == channel.Id) return;
            if (player.State != PlayerState.Playing || !channel.Users.All(x => x.IsBot)) return;

            _logger.LogInformation(LogEvent, "All users left voice in {Guild}, pausing playback...", e.Guild.Name);
            await player.PauseAsync().ConfigureAwait(false);

            if (_commandChannels.TryGetValue(e.Guild.Id, out var commandChannel))
                await commandChannel.SendMessageAsync("All users left the channel, playback paused. You can resume it by joining the channel and using the `/music resume` command.").ConfigureAwait(false);
        }

        private async Task OnTrackExceptionAsync(object sender, TrackExceptionEventArgs e)
        {
            _logger.LogError(LogEvent, "Exception occurred during audio playback: {Message}", e.Exception.Message);
            if (!_commandChannels.TryGetValue(e.Player.GuildId, out var channel)) return;
            await channel.SendMessageAsync($"A problem occurred while playing {Formatter.Bold(Formatter.Sanitize(e.Track.Title))} by {Formatter.Bold(Formatter.Sanitize(e.Track.Author))}:\n{e.Exception.Message}").ConfigureAwait(false);
        }

        public static string ToDurationString(TimeSpan ts)
        {
            return ts.ToString(ts.TotalHours >= 1 ? @"h\:mm\:ss" : @"m\:ss");
        }

        public static string DescribeTrack(LavalinkTrack track)
        {
            return $"{Formatter.Bold(Formatter.Sanitize(track.Title))} by {Formatter.Bold(Formatter.Sanitize(track.Author))}";
        }
    }
}
