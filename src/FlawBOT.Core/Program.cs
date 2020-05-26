using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using FlawBOT.Common;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using FlawBOT.Modules;

namespace FlawBOT
{
    public class Program
    {
        private static readonly object _lock = new object();
        public DiscordClient Client { get; set; }
        public CommandsNextExtension Commands { get; private set; }

        public static async Task Main(string[] args)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var app = new Program();
                app.RunBotAsync().GetAwaiter().GetResult();
                await Task.Delay(Timeout.Infinite).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nException occurred: {ex.GetType()} :\n{ex.Message}");
                if (!(ex.InnerException is null))
                    Console.WriteLine($"Inner exception: {ex.InnerException.GetType()} :\n{ex.InnerException.Message}");
                Console.ReadKey();
            }

            Console.WriteLine("\nShutting down...");
        }

        public async Task RunBotAsync()
        {
            var service = new BotServices();
            service.LoadBotConfiguration();

            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = TokenHandler.Tokens.DiscordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = false,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                LargeThreshold = 250
            });
            Client.Ready += Client_Ready;
            Client.ClientErrored += Client_ClientError;
            Client.DebugLogger.LogMessageReceived += Client_LogMessageHandler;
            Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.Ignore, // Default pagination behavior to just ignore the reactions
                Timeout = TimeSpan.FromMinutes(2) // Default pagination timeout to 2 minutes
            });

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                PrefixResolver = PrefixResolverAsync, // Set the command prefix that will be used by the bot
                EnableDms = false, // Set the boolean for responding to direct messages
                EnableMentionPrefix = true, // Set the boolean for mentioning the bot as a command prefix
                CaseSensitive = false
            });
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;
            Commands.RegisterCommands<AmiiboModule>();
            Commands.RegisterCommands<BotModule>();
            Commands.RegisterCommands<ChannelModule>();
            Commands.RegisterCommands<DictionaryModule>();
            Commands.RegisterCommands<EmojiModule>();
            Commands.RegisterCommands<GoogleModule>();
            Commands.RegisterCommands<ImgurModule>();
            Commands.RegisterCommands<MathModule>();
            Commands.RegisterCommands<MiscModule>();
            Commands.RegisterCommands<NASAModule>();
            Commands.RegisterCommands<OMDBModule>();
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
            Commands.SetHelpFormatter<HelpFormatter>();

            // Start the uptime counter
            Console.Title = SharedData.Name + $" ({SharedData.Version})";
            SharedData.ProcessStarted = DateTime.Now;
            await SteamService.UpdateSteamListAsync().ConfigureAwait(false);
            await TeamFortressService.UpdateTF2SchemaAsync().ConfigureAwait(false);
            await PokemonService.UpdatePokemonListAsync().ConfigureAwait(false);
            await Client.ConnectAsync().ConfigureAwait(false); // Connect and log into Discord
            await Task.Delay(-1).ConfigureAwait(false); // Prevent the console window from closing
        }

        private static Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, SharedData.Name, $"{SharedData.Name}, version: {SharedData.Version}", DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task Client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, SharedData.Name, $"Exception occurred: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, SharedData.Name, $"'{e.Command.QualifiedName}' executed by {e.Context.User.Username} from {e.Context.Guild.Name} : {e.Context.Channel.Name}", DateTime.Now);
            return Task.CompletedTask;
        }

        private static async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
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

                                    case RequireUserPermissionsAttribute uperms:
                                        await BotServices.SendEmbedAsync(e.Context,
                                            $"- You do not have sufficient permissions ({uperms.Permissions.ToPermissionString()})!",
                                            EmbedType.Error).ConfigureAwait(false);
                                        break;

                                    case RequireBotPermissionsAttribute bperms:
                                        await BotServices.SendEmbedAsync(e.Context,
                                            $"- I do not have sufficient permissions ({bperms.Permissions.ToPermissionString()})!",
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
                        e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, SharedData.Name,
                            $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}",
                            DateTime.Now); // DEBUG ONLY
                    break;
            }
        }

        private static Task<int> PrefixResolverAsync(DiscordMessage m)
        {
            return Task.FromResult(m.GetStringPrefixLength(TokenHandler.Tokens.CommandPrefix));
        }

        private static void Client_LogMessageHandler(object sender, DebugLogMessageEventArgs ea)
        {
            lock (_lock)
            {
                Console.BackgroundColor = ConsoleColor.Black;

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[{0:yyyy-MM-dd HH:mm:ss zzz}] ", ea.Timestamp);

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[{0}] ", ea.Application);

                var ccfg = ConsoleColor.Gray;
                var ccbg = ConsoleColor.Black;
                switch (ea.Level)
                {
                    case LogLevel.Critical:
                        ccfg = ConsoleColor.Black;
                        ccbg = ConsoleColor.Red;
                        break;

                    case LogLevel.Error:
                        ccfg = ConsoleColor.Red;
                        break;

                    case LogLevel.Warning:
                        ccfg = ConsoleColor.Yellow;
                        break;

                    case LogLevel.Info:
                        ccfg = ConsoleColor.Cyan;
                        break;

                    case LogLevel.Debug:
                        ccfg = ConsoleColor.Magenta;
                        break;
                }

                Console.ForegroundColor = ccfg;
                Console.BackgroundColor = ccbg;
                Console.Write("[{0}]", ea.Level.ToString());

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" ");
                Console.WriteLine(ea.Message);
            }
        }
    }
}