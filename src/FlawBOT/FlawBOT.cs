using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.VoiceNext;
using Emzi0767;
using FlawBOT.Common;
using FlawBOT.Modules;
using FlawBOT.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlawBOT
{
    internal sealed class FlawBot
    {
        public FlawBot(int shardId = 0)
        {
            // Setup Client
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = SharedData.Tokens.DiscordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                ReconnectIndefinitely = true,
                MinimumLogLevel = LogLevel.Information,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                LargeThreshold = 250,
                MessageCacheSize = 2048,
                LogTimestampFormat = "yyyy-MM-dd HH:mm:ss zzz",
                ShardId = shardId,
                ShardCount = SharedData.ShardCount
            });
            Client.Ready += Client_Ready;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.ClientErrored += Client_ClientErrored;
            Client.SocketErrored += Client_SocketErrored;
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
            Commands.CommandErrored += Command_Errored;
            Commands.SetHelpFormatter<HelpFormatter>();
            Commands.RegisterCommands<AmiiboModule>();
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
            Commands.RegisterCommands<OmdbModule>();
            Commands.RegisterCommands<PokemonModule>();
            Commands.RegisterCommands<PollModule>();
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
            Commands.RegisterCommands<WorldModule>();
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

            // Setup Lavalink
            Lavalink = Client.UseLavalink();
            //Process.Start("java", $"-jar {Directory.GetCurrentDirectory()}\\Lavalink.jar");

            // Start the uptime counter
            Console.Title = $"{SharedData.Name}-{SharedData.Version}";
            SharedData.ProcessStarted = DateTime.Now;
        }

        private IServiceProvider Services { get; }
        private static EventId EventId { get; } = new EventId(1000, SharedData.Name);
        private DiscordClient Client { get; }
        private CommandsNextExtension Commands { get; }
        private InteractivityExtension Interactivity { get; }
        private VoiceNextExtension Voice { get; }
        private LavalinkExtension Lavalink { get; }

        public async Task RunAsync()
        {
            // Update any other services that are being used.
            Client.Logger.LogInformation(EventId, "Loading...");
            await SteamService.UpdateSteamAppListAsync().ConfigureAwait(false);
            await TeamFortressService.UpdateTf2SchemaAsync().ConfigureAwait(false);
            await PokemonService.UpdatePokemonListAsync().ConfigureAwait(false);

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
            sender.Logger.LogInformation(EventId, $"{SharedData.Name}, version: {SharedData.Version}");
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
        {
            sender.Logger.LogInformation(EventId, $"Connected to server: {e.Guild.Name}");
            return Task.CompletedTask;
        }

        private static Task Client_ClientErrored(DiscordClient sender, ClientErrorEventArgs e)
        {
            sender.Logger.LogError(EventId, $"[{e.Exception.GetType()}] Client Exception. {e.Exception.Message}");
            return Task.CompletedTask;
        }

        private Task Client_SocketErrored(DiscordClient sender, SocketErrorEventArgs e)
        {
            var ex = e.Exception;
            while (ex is AggregateException)
                ex = ex.InnerException;

            sender.Logger.LogCritical(EventId, $"Socket threw an exception {ex.GetType()}: {ex.Message}");
            return Task.CompletedTask;
        }

        private async Task Client_VoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            var musicData = await Services.GetService<MusicService>().GetOrCreateDataAsync(e.Guild);
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
                    await musicData.CommandChannel.SendMessageAsync(
                            "All users left the channel, playback paused. You can resume it by joining the channel and using the `resume` command.")
                        .ConfigureAwait(false);
            }
        }

        private static Task Command_Executed(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            e.Context.Client.Logger.LogInformation(EventId,
                $"[{e.Context.Guild.Name} : {e.Context.Channel.Name}] {e.Context.User.Username} executed the command '{e.Command.QualifiedName}'");
            return Task.CompletedTask;
        }

        private static async Task Command_Errored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            await Exceptions.Process(e, EventId);
        }

        private static Task<int> PrefixResolverAsync(DiscordMessage m)
        {
            return Task.FromResult(m.GetStringPrefixLength(SharedData.Tokens.CommandPrefix));
        }
    }
}