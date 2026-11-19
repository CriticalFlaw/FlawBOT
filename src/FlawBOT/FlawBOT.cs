using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using FlawBOT.Common;
using FlawBOT.Modules;
using FlawBOT.Properties;
using FlawBOT.Services;
using Lavalink4NET;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FlawBOT
{
    internal sealed class FlawBot
    {
        public FlawBot(int shardId = 0)
        {
            var settings = Program.Settings;

            // Shared logging for DSharpPlus and Lavalink4NET.
            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder
                .SetMinimumLevel(LogLevel.Information)
                .AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss zzz ";
                }));

            // Setup Client
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = settings.Tokens.DiscordToken,
                TokenType = TokenType.Bot,
                // Only unprivileged intents are requested so the bot can connect without extra portal settings.
                Intents = DiscordIntents.AllUnprivileged,
                AutoReconnect = true,
                ReconnectIndefinitely = true,
                LoggerFactory = LoggerFactory,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                LargeThreshold = 250,
                MessageCacheSize = 2048,
                ShardId = shardId,
                ShardCount = settings.ShardCount
            });
            Client.Ready += Client_Ready;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.ClientErrored += Client_ClientError;
            Client.SocketErrored += Client_SocketError;

            // Setup Services
            var services = new ServiceCollection()
                .AddSingleton(LoggerFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddSingleton(Client)
                .AddSingleton<YoutubeService>()
                .AddSingleton(this);

            if (settings.Lavalink.Enabled)
            {
                services.AddLavalink();
                services.ConfigureLavalink(options =>
                {
                    options.BaseAddress = new Uri($"http://{settings.Lavalink.Address}:{settings.Lavalink.Port}");
                    options.Passphrase = settings.Lavalink.Password;
                    options.ReadyTimeout = TimeSpan.FromSeconds(15);
                    options.Label = $"{settings.Name}-{shardId}";
                });
                services.AddSingleton<MusicService>();
            }

            Services = services.BuildServiceProvider(true);

            // Setup Slash Commands
            Slash = Client.UseSlashCommands(new SlashCommandsConfiguration { Services = Services });
            Slash.RegisterCommands<BotModule>();
            Slash.RegisterCommands<ChannelModule>();
            Slash.RegisterCommands<DictionaryModule>();
            Slash.RegisterCommands<ImgurModule>();
            Slash.RegisterCommands<MiscModule>();
            Slash.RegisterCommands<NasaModule>();
            Slash.RegisterCommands<NewsModule>();
            Slash.RegisterCommands<NintendoModule>();
            Slash.RegisterCommands<OmdbModule>();
            Slash.RegisterCommands<RedditModule>();
            Slash.RegisterCommands<RoleModule>();
            Slash.RegisterCommands<ServerModule>();
            Slash.RegisterCommands<SimpsonsModule>();
            Slash.RegisterCommands<SpeedrunModule>();
            Slash.RegisterCommands<SteamModule>();
            Slash.RegisterCommands<TeamFortressModule>();
            Slash.RegisterCommands<TwitchModule>();
            Slash.RegisterCommands<UserModule>();
            Slash.RegisterCommands<WeatherModule>();
            Slash.RegisterCommands<WikipediaModule>();
            Slash.RegisterCommands<YouTubeModule>();
            if (settings.Lavalink.Enabled)
            {
                Slash.RegisterCommands<MusicModule>();
                Client.VoiceStateUpdated += (sender, e) => Services.GetRequiredService<MusicService>().HandleVoiceStateUpdatedAsync(sender, e);
            }

            Slash.SlashCommandInvoked += SlashCommand_Invoked;
            Slash.SlashCommandErrored += SlashCommand_Errored;

            // Set the console title (only meaningful when attached to a Windows console).
            if (OperatingSystem.IsWindows() && !Console.IsOutputRedirected)
                Console.Title = $"{settings.Name}-{settings.Version}";
        }

        private ILoggerFactory LoggerFactory { get; }
        internal ServiceProvider Services { get; }
        private static EventId EventId { get; } = new(1000, "FlawBOT");
        private DiscordClient Client { get; }
        internal SlashCommandsExtension Slash { get; }

        public async Task RunAsync()
        {
            Client.Logger.LogInformation(EventId, "Initializing shard {ShardId}...", Client.ShardId);

            // Set the initial activity and connect the bot to Discord
            var act = new DiscordActivity(Resources.APP_DEFAULT_ACTIVITY, ActivityType.ListeningTo);
            await Client.ConnectAsync(act, UserStatus.DoNotDisturb).ConfigureAwait(false);

            if (Program.Settings.Lavalink.Enabled)
                _ = StartLavalinkAsync();
        }

        public async Task StopAsync()
        {
            try
            {
                var audio = Services.GetService<IAudioService>();
                if (audio is not null) await audio.StopAsync().ConfigureAwait(false);
                await Client.DisconnectAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Client.Logger.LogWarning(EventId, ex, "Error while disconnecting.");
            }
            finally
            {
                await Services.DisposeAsync().ConfigureAwait(false);
                LoggerFactory.Dispose();
            }
        }

        private async Task StartLavalinkAsync()
        {
            try
            {
                Client.Logger.LogInformation(EventId, "Connecting to Lavalink at {Address}:{Port}...", Program.Settings.Lavalink.Address, Program.Settings.Lavalink.Port);
                await Services.GetRequiredService<IAudioService>().StartAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Client.Logger.LogError(EventId, ex, Resources.ERR_LAVALINK_CONNECTION);
            }
        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            var settings = Program.Settings;
            sender.Logger.LogInformation(EventId, "{Name}, version {Version}", settings.Name, settings.Version);
            return Task.CompletedTask;
        }

        private static Task Client_GuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
        {
            sender.Logger.LogInformation(EventId, "Connected to server: {Guild}", e.Guild.Name);
            return Task.CompletedTask;
        }

        private static Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs e)
        {
            sender.Logger.LogError(EventId, e.Exception, "Client exception in event {Event}.", e.EventName);
            return Task.CompletedTask;
        }

        private static Task Client_SocketError(DiscordClient sender, SocketErrorEventArgs e)
        {
            var ex = e.Exception;
            while (ex is AggregateException)
                ex = ex.InnerException;

            sender.Logger.LogCritical(EventId, "Socket threw an exception {Type}: {Message}", ex?.GetType(), ex?.Message);
            return Task.CompletedTask;
        }

        private static Task SlashCommand_Invoked(SlashCommandsExtension sender, SlashCommandInvokedEventArgs e)
        {
            e.Context.Client.Logger.LogInformation(EventId, "[{Guild} : {Channel}] {User} executed the command '{Command}'",
                e.Context.Guild?.Name ?? "DM", e.Context.Channel?.Name, e.Context.User.Username, e.Context.QualifiedName);
            return Task.CompletedTask;
        }

        private static Task SlashCommand_Errored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
        {
            return Exceptions.Process(e, EventId);
        }
    }
}
