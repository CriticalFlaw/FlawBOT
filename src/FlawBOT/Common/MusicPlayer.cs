using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using FlawBOT.Models;
using FlawBOT.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlawBOT.Common
{
    public sealed class MusicPlayer
    {
        public MusicPlayer(LavalinkService lavalink)
        {
            LavaLink = lavalink;
            QueueLock = new SemaphoreSlim(1, 1);
            QueueList = new List<MusicData>();
        }

        private LavalinkService LavaLink { get; }
        private SemaphoreSlim QueueLock { get; }
        private List<MusicData> QueueList { get; }
        private LavalinkGuildConnection Player { get; set; }

        public bool IsPlaying { get; private set; }
        public int Volume { get; private set; } = 100;
        public MusicData NowPlaying { get; private set; }
        public DiscordChannel Channel => Player?.Channel;
        public DiscordChannel CommandChannel { get; set; }

        public async Task PlayAsync()
        {
            if (Player == null || !Player.IsConnected) return;

            if (NowPlaying.Track?.TrackString == null)
                await PlayHandlerAsync();
        }

        public async Task StopAsync()
        {
            if (Player == null || !Player.IsConnected) return;

            QueueList.Clear();
            NowPlaying = default;
            IsPlaying = false;
            await Player.StopAsync();
        }

        public async Task PauseAsync()
        {
            if (Player == null || !Player.IsConnected) return;

            IsPlaying = false;
            await Player.PauseAsync();
        }

        public async Task ResumeAsync()
        {
            if (Player == null || !Player.IsConnected) return;

            IsPlaying = true;
            await Player.ResumeAsync();
        }

        public async Task SetVolumeAsync(int volume)
        {
            if (Player == null || !Player.IsConnected) return;

            await Player.SetVolumeAsync(volume);
            Volume = volume;
        }

        public async Task RestartAsync()
        {
            if (Player == null || !Player.IsConnected || NowPlaying.Track.TrackString == null) return;

            await QueueLock.WaitAsync();
            QueueList.Insert(0, NowPlaying);
            await Player.StopAsync();
            QueueLock.Release();
        }

        public void Enqueue(MusicData item)
        {
            lock (QueueList)
            {
                if (QueueList.Count == 1 || IsPlaying)
                {
                    Player.StopAsync();
                    QueueList.Clear();
                }

                if (!QueueList.Any())
                    QueueList.Add(item);
            }
        }

        public MusicData? Dequeue()
        {
            lock (QueueList)
            {
                if (QueueList.Count == 0) return null;

                var item = QueueList[0];
                QueueList.RemoveAt(0);
                return item;
            }
        }

        public async Task CreatePlayerAsync(DiscordChannel channel)
        {
            if (Player is { IsConnected: true }) return;

            Player = await LavaLink.Node.ConnectAsync(channel);

            if (Volume != 100)
                await Player.SetVolumeAsync(Volume);
            Player.PlaybackFinished += Player_PlaybackFinished;
        }

        public async Task DestroyPlayerAsync()
        {
            if (Player == null) return;

            if (Player.IsConnected)
                await Player.DisconnectAsync();

            Player = null;
        }

        public TimeSpan GetCurrentPosition()
        {
            return NowPlaying.Track.TrackString == null ? TimeSpan.Zero : Player.CurrentState.PlaybackPosition;
        }

        private async Task Player_PlaybackFinished(LavalinkGuildConnection con, TrackFinishEventArgs e)
        {
            await Task.Delay(500);
            IsPlaying = false;
            await PlayHandlerAsync();
        }

        private async Task PlayHandlerAsync()
        {
            var song = Dequeue();
            if (song == null)
            {
                NowPlaying = default;
                return;
            }

            var item = song.Value;
            NowPlaying = item;
            IsPlaying = true;
            await Player.PlayAsync(item.Track);
        }
    }
}