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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlawBOT.Services;
using FlawBOT.Modules;
using DSharpPlus.SlashCommands.EventArgs;

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

            // Setup Interactivity
            Interactivity = Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.Ignore,
                Timeout = TimeSpan.FromSeconds(30),
                AckPaginationButtons = true
            });

            // Setup Voice
            Voice = Client.UseVoiceNext(new VoiceNextConfiguration
            {
                AudioFormat = AudioFormat.Default,
                EnableIncoming = true
            });

            // Setup Slash Commands
            Slash = Client.UseSlashCommands();
            Slash.RegisterCommands<BotModule>();
            Slash.RegisterCommands<ChannelModule>();
            Slash.RegisterCommands<DictionaryModule>();
            //Slash.RegisterCommands<EmojiModule>();
            Slash.RegisterCommands<ImgurModule>();
            Slash.RegisterCommands<MiscModule>();
            Slash.RegisterCommands<MusicModule>();
            Slash.RegisterCommands<NasaModule>();
            Slash.RegisterCommands<NewsModule>();
            Slash.RegisterCommands<NintendoModule>();
            Slash.RegisterCommands<OmdbModule>();
            Slash.RegisterCommands<RedditModule>();
            //Slash.RegisterCommands<RoleModule>();
            //Slash.RegisterCommands<ServerModule>();
            Slash.RegisterCommands<SimpsonsModule>();
            Slash.RegisterCommands<SpeedrunModule>();
            Slash.RegisterCommands<SteamModule>();
            Slash.RegisterCommands<TeamFortressModule>();
            Slash.RegisterCommands<TwitchModule>();
            //Slash.RegisterCommands<UserModule>();
            Slash.RegisterCommands<WeatherModule>();
            Slash.RegisterCommands<WikipediaModule>();
            Slash.RegisterCommands<YoutuberModule>();
            Slash.SlashCommandInvoked += SlashCommand_Executed;
            Slash.SlashCommandErrored += SlashCommand_Errored;

            // Setup Lavalink
            if (settings.Lavalink.Enabled)
            {
                if (File.Exists($"{Directory.GetCurrentDirectory()}/Lavalink.jar"))
                {
                    Client.Logger.LogInformation(EventId, "Initializing Lavalink...");
                    Lavalink = Client.UseLavalink();
                }
                else
                {
                    Client.Logger.LogInformation(EventId, $"Could not find Lavalink node at: {Directory.GetCurrentDirectory()}");
                }
            }

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
            //await SteamService.UpdateSteamAppListAsync(Program.Settings.Tokens.SteamToken).ConfigureAwait(false);
            //await TeamFortressService.UpdateTF2SchemaAsync(Program.Settings.Tokens.SteamToken).ConfigureAwait(false);

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

        private static Task SlashCommand_Executed(SlashCommandsExtension sender, SlashCommandInvokedEventArgs e)
        {
            e.Context.Client.Logger.LogInformation($"{e.Context.User.Username} successfully executed '{e.Context.CommandName}'");
            return Task.CompletedTask;
        }

        private static async Task Command_Error(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            await Exceptions.Process(e, EventId);
        }

        private static async Task SlashCommand_Errored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
        {
            await Exceptions.Process(e, EventId);
        }

        private static Task<int> PrefixResolverAsync(DiscordMessage m)
        {
            return Task.FromResult(m.GetStringPrefixLength(Program.Settings.Prefix));
        }
    }
}