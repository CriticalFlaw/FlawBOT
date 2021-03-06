﻿using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using DSharpPlus.Net;
using Emzi0767.Utilities;
using Microsoft.Extensions.Logging;

namespace FlawBOT.Services
{
    public sealed class LavalinkService
    {
        private readonly AsyncEvent<LavalinkGuildConnection, TrackExceptionEventArgs> _trackException;

        public LavalinkService(DiscordClient client)
        {
            Discord = client;
            Discord.Ready += Client_Ready;
            _trackException =
                new AsyncEvent<LavalinkGuildConnection, TrackExceptionEventArgs>(
                    $"{Program.Settings.Name.ToUpperInvariant()}_LAVALINK_TRACK_EXCEPTION",
                    TimeSpan.Zero, EventExceptionHandler);
        }

        private DiscordClient Discord { get; }

        public static EventId LogEvent { get; } = new(1001, Program.Settings.Name);

        public LavalinkNodeConnection Node { get; private set; }

        private Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            if (Node == null)
                _ = Task.Run(async () =>
                {
                    var lava = sender.GetLavalink();
                    if (lava is not null)
                    {
                        Node = await lava.ConnectAsync(new LavalinkConfiguration
                        {
                            Password = "youshallnotpass",
                            SocketEndpoint = new ConnectionEndpoint("192.168.2.38", 2333),
                            RestEndpoint = new ConnectionEndpoint("192.168.2.38", 2333)
                        });

                        Node.TrackException += LavalinkNode_TrackException;
                    }
                });

            return Task.CompletedTask;
        }

        private async Task LavalinkNode_TrackException(LavalinkGuildConnection con, TrackExceptionEventArgs e)
        {
            await _trackException.InvokeAsync(con, e);
        }

        public event AsyncEventHandler<LavalinkGuildConnection, TrackExceptionEventArgs> TrackExceptionThrown
        {
            add => _trackException.Register(value);
            remove => _trackException.Unregister(value);
        }

        private void EventExceptionHandler(
            AsyncEvent<LavalinkGuildConnection, TrackExceptionEventArgs> asyncEvent,
            Exception exception,
            AsyncEventHandler<LavalinkGuildConnection, TrackExceptionEventArgs> handler,
            LavalinkGuildConnection sender,
            TrackExceptionEventArgs eventArgs)
        {
            Discord.Logger.LogError(LogEvent, exception, "Exception occurred during audio playback.");
        }
    }
}