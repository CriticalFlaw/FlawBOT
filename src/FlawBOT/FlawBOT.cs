using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.VoiceNext;
using DSharpPlus.SlashCommands;
using Emzi0767;
using FlawBOT.Common;
using FlawBOT.Modules.Bot;
using FlawBOT.Modules.Games;
using FlawBOT.Modules.Misc;
using FlawBOT.Modules.Search;
using FlawBOT.Modules.Server;
using FlawBOT.Services;
using FlawBOT.Services.Games;
using FlawBOT.Services.Lookup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT
{
    internal sealed class FlawBot
    {
        public FlawBot(int shardId = 0)
        {
            // Get Settings
            var settings = Program.Settings;

            // Setup Client
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = settings.Tokens.DiscordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                ReconnectIndefinitely = true,
                MinimumLogLevel = LogLevel.Information,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                LargeThreshold = 250,
                MessageCacheSize = 2048,
                LogTimestampFormat = "yyyy-MM-dd HH:mm:ss zzz",
                ShardId = shardId,
                ShardCount = settings.ShardCount
            });
            Client.Ready += Client_Ready;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.ClientErrored += Client_ClientError;
            Client.SocketErrored += Client_SocketError;
            Client.VoiceStateUpdated += Client_VoiceStateUpdated;

            // Setup Services
            Services = new ServiceCollection()
                .AddTransient<SecureRandom>()
                .AddSingleton<MusicService>()
                .AddSingleton(new LavalinkService(Client))
                .AddSingleton(new YoutubeService())
                .AddSingleton(this)
                .BuildServiceProvider(true);

            // Setup Commands
            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                PrefixResolver = PrefixResolverAsync, // Set the command prefix that will be used by the bot
                EnableDms = false, // Set the boolean for responding to direct messages
                EnableMentionPrefix = true, // Set the boolean for mentioning the bot as a command prefix
                CaseSensitive = false,
                IgnoreExtraArguments = true,
                Services = Services
            });
            Commands.CommandExecuted += Command_Executed;
            Commands.CommandErrored += Command_Error;
            Commands.SetHelpFormatter<HelpFormatter>();
            Commands.RegisterCommands<BotModule>();
            Commands.RegisterCommands<ChannelModule>();
            Commands.RegisterCommands<DictionaryModule>();
            Commands.RegisterCommands<EmojiModule>();
            Commands.RegisterCommands<ImgurModule>();
            Commands.RegisterCommands<MathModule>();
            Commands.RegisterCommands<MiscModule>();
            Commands.RegisterCommands<MusicModule>();
            Commands.RegisterCommands<NasaModule>();
            Commands.RegisterCommands<NewsModule>();
            Commands.RegisterCommands<NintendoModule>();
            Commands.RegisterCommands<OmdbModule>();
            Commands.RegisterCommands<RedditModule>();
            Commands.RegisterCommands<RoleModule>();
            Commands.RegisterCommands<ServerModule>();
            Commands.RegisterCommands<SimpsonsModule>();
            Commands.RegisterCommands<SpeedrunModule>();
            Commands.RegisterCommands<SteamModule>();
            Commands.RegisterCommands<TeamFortressModule>();
            Commands.RegisterCommands<TwitchModule>();
            Commands.RegisterCommands<UserModule>();
            Commands.RegisterCommands<WikipediaModule>();
            Commands.RegisterCommands<WeatherModule>();
            Commands.RegisterCommands<YouTubeModule>();

            // Setup Interactivity
            Interactivity = Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.Ignore,
                Timeout = TimeSpan.FromSeconds(30)
            });

            // Setup Voice
            Voice = Client.UseVoiceNext(new VoiceNextConfiguration
            {
                AudioFormat = AudioFormat.Default,
                EnableIncoming = true
            });

            // Setup Slash Commands
            Slash = Client.UseSlashCommands();
            Slash.RegisterCommands<SlashCommands>();

            // Setup Lavalink
            var output = "Lavalink node not enabled. Skipping...";
            if (settings.Lavalink.Enabled)
            {
                if (File.Exists($"{Directory.GetCurrentDirectory()}/Lavalink.jar"))
                {
                    output = "Lavalink enabled. Initializing .jar node...";
                    Lavalink = Client.UseLavalink();
                }
                else
                {
                    output = $"Could not find Lavalink node at: {Directory.GetCurrentDirectory()}";
                }
            }

            Client.Logger.LogInformation(EventId, output);

            // Start the uptime counter
            Console.Title = $"{settings.Name}-{settings.Version}";
            settings.ProcessStarted = DateTime.Now;
        }

        private IServiceProvider Services { get; }
        private static EventId EventId { get; } = new(1000, Program.Settings.Name);
        private DiscordClient Client { get; }
        private CommandsNextExtension Commands { get; }
        private InteractivityExtension Interactivity { get; }
        private VoiceNextExtension Voice { get; }
        private LavalinkExtension Lavalink { get; }
        private SlashCommandsExtension Slash { get; }

        public async Task RunAsync()
        {
            // Update any other services that are being used.
            Client.Logger.LogInformation(EventId, "Initializing...");
            await SteamService.UpdateSteamAppListAsync(Program.Settings.Tokens.SteamToken).ConfigureAwait(false);
            await TeamFortressService.UpdateTf2SchemaAsync(Program.Settings.Tokens.SteamToken).ConfigureAwait(false);

            // Send a notification to load Lavalink
            Client.Logger.LogInformation(EventId, "Make sure Lavalink is running!");

            // Set the initial activity and connect the bot to Discord
            var act = new DiscordActivity("Night of Fire", ActivityType.ListeningTo);
            await Client.ConnectAsync(act, UserStatus.DoNotDisturb).ConfigureAwait(false);
        }

        public async Task StopAsync()
        {
            await Client.DisconnectAsync().ConfigureAwait(false);
        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            var settings = Program.Settings;
            sender.Logger.LogInformation(EventId, $"{settings.Name}, version {settings.Version}");
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
        {
            sender.Logger.LogInformation(EventId, $"Connected to server: {e.Guild.Name}");
            return Task.CompletedTask;
        }

        private static Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs e)
        {
            sender.Logger.LogError(EventId, $"[{e.Exception.GetType()}] Client Exception. {e.Exception.Message}");
            return Task.CompletedTask;
        }

        private Task Client_SocketError(DiscordClient sender, SocketErrorEventArgs e)
        {
            var ex = e.Exception;
            while (ex is AggregateException)
                ex = ex.InnerException;

            sender.Logger.LogCritical(EventId, $"Socket threw an exception {ex?.GetType()}: {ex?.Message}");
            return Task.CompletedTask;
        }

        private async Task Client_VoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            var musicData = await Services.GetService<MusicService>()?.GetOrCreateDataAsync(e.Guild)!;
            if (e.After.Channel == null && e.User == Client.CurrentUser)
            {
                await musicData.StopAsync();
                await musicData.DestroyPlayerAsync();
                return;
            }

            if (e.User == Client.CurrentUser) return;
            var channel = musicData.Channel;
            if (channel == null || channel != e.Before.Channel) return;

            var users = channel.Users;
            if (musicData.IsPlaying && users.All(x => x.IsBot))
            {
                sender.Logger.LogInformation(EventId, $"All users left voice in {e.Guild.Name}, pausing playback...");
                await musicData.PauseAsync();

                if (musicData.CommandChannel != null)
                    await musicData.CommandChannel.SendMessageAsync("All users left the channel, playback paused. You can resume it by joining the channel and using the `resume` command.").ConfigureAwait(false);
            }
        }

        private static Task Command_Executed(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            e.Context.Client.Logger.LogInformation(EventId, $"[{e.Context.Guild.Name} : {e.Context.Channel.Name}] {e.Context.User.Username} executed the command '{e.Command.QualifiedName}'");
            return Task.CompletedTask;
        }

        private static async Task Command_Error(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            await Exceptions.Process(e, EventId);
        }

        private static Task<int> PrefixResolverAsync(DiscordMessage m)
        {
            return Task.FromResult(m.GetStringPrefixLength(Program.Settings.Prefix));
        }
    }
}