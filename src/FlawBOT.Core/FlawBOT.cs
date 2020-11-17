using System;
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
using FlawBOT.Common;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using FlawBOT.Modules;
using Microsoft.Extensions.Logging;

namespace FlawBOT
{
    internal sealed class FlawBot
    {
        public FlawBot(int shardId = 0)
        {
            //var depot = new ServiceCollection();

            // Setup Client
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = TokenHandler.Tokens.DiscordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Information,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                LargeThreshold = 250,
                MessageCacheSize = 2048,
                LogTimestampFormat = "yyyy-MM-dd HH:mm:ss zzz",
                ShardId = shardId
                //ShardCount = 0
            });
            Client.Ready += Client_Ready;
            //Client.GuildAvailable += Client_GuildAvailable;
            Client.ClientErrored += Client_Error;
            //Client.SocketErrored += Client_SocketError;
            //Client.GuildCreated += Client_GuildCreated;
            Client.VoiceStateUpdated += Client_VoiceStateUpdated;
            //Client.GuildDownloadCompleted += Client_GuildDownloadCompleted;
            //Client.GuildUpdated += Client_GuildUpdated;
            //Client.ChannelDeleted += Client_ChannelDeleted;

            // Setup Commands
            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                PrefixResolver = PrefixResolverAsync, // Set the command prefix that will be used by the bot
                EnableDms = false, // Set the boolean for responding to direct messages
                EnableMentionPrefix = true, // Set the boolean for mentioning the bot as a command prefix
                CaseSensitive = false
                //StringPrefixes = null,
                //Services = depot.BuildServiceProvider(true),
                //IgnoreExtraArguments = false,
                //UseDefaultCommandHandler = false
            });
            Commands.CommandExecuted += Commands_Executed;
            Commands.CommandErrored += Commands_Error;
            Commands.SetHelpFormatter<HelpFormatter>();
            Commands.RegisterCommands<AmiiboModule>();
            Commands.RegisterCommands<BotModule>();
            Commands.RegisterCommands<ChannelModule>();
            Commands.RegisterCommands<DictionaryModule>();
            Commands.RegisterCommands<EmojiModule>();
            Commands.RegisterCommands<GoogleModule>();
            Commands.RegisterCommands<ImgurModule>();
            Commands.RegisterCommands<MathModule>();
            Commands.RegisterCommands<MiscModule>();
            Commands.RegisterCommands<NasaModule>();
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
            Commands.RegisterCommands<YouTubeModule>();
            Commands.RegisterCommands<VoiceModule>();

            // Setup Interactivity
            Interactivity = Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.Ignore,
                Timeout = TimeSpan.FromMinutes(2)
            });

            // Setup Voice
            Voice = Client.UseVoiceNext(new VoiceNextConfiguration
            {
                AudioFormat = AudioFormat.Default,
                EnableIncoming = true
            });

            // Setup Lavalink
            Lavalink = Client.UseLavalink();

            // Start the uptime counter
            Console.Title = SharedData.Name + "-" + SharedData.Version;
            SharedData.ProcessStarted = DateTime.Now;
        }

        private static EventId EventId { get; } = new EventId(1000, "FlawBot");
        private DiscordClient Client { get; }
        private CommandsNextExtension Commands { get; }
        private InteractivityExtension Interactivity { get; }
        private VoiceNextExtension Voice { get; }
        private LavalinkExtension Lavalink { get; }

        private Task Client_VoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            Client.Logger.LogDebug(EventId, "Voice state changed for '{0}' (mute: {1} -> {2}; deaf: {3} -> {4})",
                e.User, e.Before?.IsServerMuted, e.After.IsServerMuted, e.Before?.IsServerDeafened,
                e.After.IsServerDeafened);
            return Task.CompletedTask;
        }

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

        private static Task Client_Error(DiscordClient sender, ClientErrorEventArgs e)
        {
            sender.Logger.LogError(EventId, $"[{e.Exception.GetType()}] Client Exception. {e.Exception.Message}");
            return Task.CompletedTask;
        }

        private static Task Commands_Executed(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            e.Context.Client.Logger.LogInformation(EventId,
                string.Format("[{0} : {1}] {2} executed the command '{3}'", e.Context.Guild.Name,
                    e.Context.Channel.Name, e.Context.User.Username, e.Command.QualifiedName));
            return Task.CompletedTask;
        }

        private static async Task Commands_Error(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            await Exceptions.Process(e, EventId);
        }

        private static Task<int> PrefixResolverAsync(DiscordMessage m)
        {
            return Task.FromResult(m.GetStringPrefixLength(TokenHandler.Tokens.CommandPrefix));
        }
    }
}