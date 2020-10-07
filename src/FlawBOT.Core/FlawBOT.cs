using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Lavalink;
using DSharpPlus.VoiceNext;
using FlawBOT.Common;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using FlawBOT.Modules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlawBOT
{
    internal sealed class FlawBOT
    {
        internal static EventId EventId { get; } = new EventId(1000, "FlawBot");
        public DiscordClient Client { get; set; }
        private CommandsNextExtension Commands { get; }
        private InteractivityExtension Interactivity { get; }
        private VoiceNextExtension Voice { get; }
        private LavalinkExtension LavaLink { get; }

        public FlawBOT(int shardId = 0)
        {
            var depot = new ServiceCollection();

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
                ShardId = shardId,
                //ShardCount = 0
            });
            Client.Ready += Client_Ready;
            //Client.GuildAvailable += Client_GuildAvailable;
            Client.ClientErrored += Client_Error;
            //Client.SocketErrored += Client_SocketError;
            //Client.GuildCreated += Client_GuildCreated;
            //Client.VoiceStateUpdated += Client_VoiceStateUpdated;
            //Client.GuildDownloadCompleted += Client_GuildDownloadCompleted;
            //Client.GuildUpdated += Client_GuildUpdated;
            //Client.ChannelDeleted += Client_ChannelDeleted;

            // Setup Commands
            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                PrefixResolver = PrefixResolverAsync, // Set the command prefix that will be used by the bot
                EnableDms = false, // Set the boolean for responding to direct messages
                EnableMentionPrefix = true, // Set the boolean for mentioning the bot as a command prefix
                CaseSensitive = false,
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
                EnableIncoming = false
            });

            // Setup LavaLink
            LavaLink = Client.UseLavalink();

            // Start the uptime counter
            Console.Title = $"{SharedData.Name}-{SharedData.Version}";
            SharedData.ProcessStarted = DateTime.Now;
        }

        public async Task RunAsync()
        {
            // Update any other services that are being used.
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

        private Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            sender.Logger.LogInformation(EventId, $"{SharedData.Name}, version: {SharedData.Version}");
            return Task.CompletedTask;
        }

        private Task Client_Error(DiscordClient sender, ClientErrorEventArgs e)
        {
            sender.Logger.LogError(EventId, $"Exception occurred: {e.Exception.GetType()}: {e.Exception.Message}");
            return Task.CompletedTask;
        }

        private Task Commands_Executed(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            e.Context.Client.Logger.LogInformation(EventId, $"'{e.Command.QualifiedName}' executed by {e.Context.User.Username} from {e.Context.Guild.Name} : {e.Context.Channel.Name}");
            return Task.CompletedTask;
        }

        private async Task Commands_Error(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if (e.Exception is CommandNotFoundException && (e.Command == null || e.Command.QualifiedName != "help")) return;

            e.Context.Client.Logger.LogError(EventId, e.Exception, "Exception occurred during {0}'s invocation of '{1}'", e.Context.User.Username, e.Context.Command.QualifiedName);

            // TO-DO: Refactor this error handler.
            switch (e.Exception)
            {
                case ChecksFailedException cfe:
                    switch (cfe.FailedChecks[0])
                    {
                        case CooldownAttribute _:
                            return;

                        default:
                            await BotServices.SendEmbedAsync(e.Context,
                                    $"Command **{e.Command.QualifiedName}** could not be executed.", EmbedType.Error)
                                .ConfigureAwait(false);
                            foreach (var check in cfe.FailedChecks)
                                switch (check)
                                {
                                    case RequirePermissionsAttribute perms:
                                        await BotServices.SendEmbedAsync(e.Context,
                                            $"- One of us does not have the required permissions ({perms.Permissions.ToPermissionString()})!",
                                            EmbedType.Error).ConfigureAwait(false);
                                        break;

                                    case RequireUserPermissionsAttribute perms:
                                        await BotServices.SendEmbedAsync(e.Context,
                                            $"- You do not have sufficient permissions ({perms.Permissions.ToPermissionString()})!",
                                            EmbedType.Error).ConfigureAwait(false);
                                        break;

                                    case RequireBotPermissionsAttribute perms:
                                        await BotServices.SendEmbedAsync(e.Context,
                                            $"- I do not have sufficient permissions ({perms.Permissions.ToPermissionString()})!",
                                            EmbedType.Error).ConfigureAwait(false);
                                        break;

                                    case RequireOwnerAttribute _:
                                        await BotServices.SendEmbedAsync(e.Context,
                                                "- This command is reserved only for the bot owner.", EmbedType.Error)
                                            .ConfigureAwait(false);
                                        break;

                                    case RequirePrefixesAttribute pa:
                                        await BotServices.SendEmbedAsync(e.Context,
                                            $"- This command can only be invoked with the following prefixes: {string.Join(" ", pa.Prefixes)}.",
                                            EmbedType.Error).ConfigureAwait(false);
                                        break;

                                    default:
                                        await BotServices.SendEmbedAsync(e.Context,
                                            "Unknown check triggered. Please notify the developer using the command *.bot report*",
                                            EmbedType.Error).ConfigureAwait(false);
                                        break;
                                }

                            break;
                    }

                    break;

                case CommandNotFoundException _:
                    //await BotServices.SendEmbedAsync(e.Context, "This command does not exist!", EmbedType.Error);
                    break;

                case NullReferenceException _:
                    //await BotServices.SendEmbedAsync(e.Context, Resources.NOT_FOUND_GENERIC, EmbedType.Missing);
                    break;

                case ArgumentNullException _:
                    await BotServices
                        .SendEmbedAsync(e.Context, "Not enough arguments supplied to the command!", EmbedType.Error)
                        .ConfigureAwait(false);
                    break;

                case ArgumentException _:
                    if (e.Exception.Message.Contains("Not enough arguments supplied to the command"))
                        await BotServices
                            .SendEmbedAsync(e.Context, "Not enough arguments supplied to the command!", EmbedType.Error)
                            .ConfigureAwait(false);
                    break;

                case InvalidDataException _:
                    if (e.Exception.Message.Contains("The data within the stream was not valid image data"))
                        await BotServices
                            .SendEmbedAsync(e.Context, "Provided URL is not an image type!", EmbedType.Error)
                            .ConfigureAwait(false);
                    break;

                default:
                    if (e.Exception.Message.Contains("Given emote was not found"))
                        await BotServices.SendEmbedAsync(e.Context, "Suggested emote was not found!", EmbedType.Error)
                            .ConfigureAwait(false);
                    if (e.Exception.Message.Contains("Unauthorized: 403"))
                        await BotServices.SendEmbedAsync(e.Context, "Insufficient Permissions", EmbedType.Error)
                            .ConfigureAwait(false);
                    else
                        e.Context.Client.Logger.LogError(EventId, $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message}"); // DEBUG ONLY
                    break;
            }
        }

        private static Task<int> PrefixResolverAsync(DiscordMessage m)
        {
            return Task.FromResult(m.GetStringPrefixLength(TokenHandler.Tokens.CommandPrefix));
        }
    }
}