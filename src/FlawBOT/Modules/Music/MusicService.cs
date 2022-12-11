using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public sealed class MusicService
    {
        public MusicService(LavalinkService lavalink)
        {
            Lavalink = lavalink;
            MusicData = new ConcurrentDictionary<ulong, MusicPlayer>();
            Lavalink.TrackExceptionThrown += Lavalink_TrackExceptionThrown;
        }

        private LavalinkService Lavalink { get; }

        private ConcurrentDictionary<ulong, MusicPlayer> MusicData { get; }

        public Task<MusicPlayer> GetOrCreateDataAsync(DiscordGuild server)
        {
            return Task.FromResult(MusicData.TryGetValue(server.Id, out var player)
                ? player
                : MusicData.AddOrUpdate(server.Id, new MusicPlayer(Lavalink), (_, v) => v));
        }

        public Task<LavalinkLoadResult> GetTracksAsync(Uri uri)
        {
            return Lavalink.Node.Rest.GetTracksAsync(uri);
        }

        private async Task Lavalink_TrackExceptionThrown(LavalinkGuildConnection con, TrackExceptionEventArgs e)
        {
            if (e.Player?.Guild == null) return;

            if (!MusicData.TryGetValue(e.Player.Guild.Id, out var gmd)) return;

            await gmd.CommandChannel.SendMessageAsync($"A problem occurred while playing {Formatter.Bold(Formatter.Sanitize(e.Track.Title))} by {Formatter.Bold(Formatter.Sanitize(e.Track.Author))}:\n{e.Error}");
        }

        public static string ToDurationString(TimeSpan ts)
        {
            return ts.ToString(ts.TotalHours >= 1 ? @"h\:mm\:ss" : @"m\:ss");
        }
    }
}