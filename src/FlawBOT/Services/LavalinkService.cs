using DSharpPlus;
using DSharpPlus.AsyncEvents;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using DSharpPlus.Net;
using FlawBOT.Properties;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public sealed class LavalinkService
    {
        private readonly AsyncEvent<LavalinkGuildConnection, TrackExceptionEventArgs> _trackException;

        public LavalinkService(DiscordClient client)
        {
            Discord = client;
            Discord.Ready += Client_Ready;
            _trackException = new AsyncEvent<LavalinkGuildConnection, TrackExceptionEventArgs>($"{Program.Settings.Name.ToUpperInvariant()}_LAVALINK_TRACK_EXCEPTION", EventExceptionHandler);
        }

        private DiscordClient Discord { get; }

        public static EventId LogEvent { get; } = new(1001, Program.Settings.Name);

        public LavalinkNodeConnection Node { get; private set; }

        private Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            if (Node is null)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var lava = sender.GetLavalink();
                        if (lava is not null)
                        {
                            Node = await lava.ConnectAsync(new LavalinkConfiguration
                            {
                                Password = Program.Settings.Lavalink.Password,
                                SocketEndpoint = new ConnectionEndpoint(Program.Settings.Lavalink.Address, Program.Settings.Lavalink.Port),
                                RestEndpoint = new ConnectionEndpoint(Program.Settings.Lavalink.Address, Program.Settings.Lavalink.Port)
                            });

                            Node.TrackException += LavalinkNode_TrackException;
                        }
                    }
                    catch (Exception ex)
                    {
                        Discord.Logger.LogError(LogEvent, ex, Resources.ERR_LAVALINK_CONNECTION);
                        throw;
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
            Discord.Logger.LogError(LogEvent, exception, Resources.ERR_LAVALINK_PLAYBACK);
        }
    }
}