using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using FlawBOT.Common;
using FlawBOT.Models;
using FlawBOT.Modules.Bot;
using FlawBOT.Modules.Games;
using FlawBOT.Modules.Misc;
using FlawBOT.Modules.Search;
using FlawBOT.Modules.Server;
using FlawBOT.Services;
using FlawBOT.Services.Games;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FlawBOT
{
    public class Program
    {
        public DiscordClient Client { get; set; }
        public CommandsNextExtension Commands { get; private set; }
        private static readonly object _lock = new object();

        public static void Main(string[] args)
        {
            var app = new Program();
            app.RunBotAsync().GetAwaiter().GetResult();
        }

        public async Task RunBotAsync()
        {
            var service = new BotServices();
            service.LoadBotConfig();

            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = GlobalVariables.config.DiscordToken,
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
                PaginationBehavior = TimeoutBehaviour.Ignore, // Default pagination behaviour to just ignore the reactions
                PaginationTimeout = TimeSpan.FromMinutes(5), // Default pagination timeout to 5 minutes
                Timeout = TimeSpan.FromMinutes(2) // Default timeout for other actions to 2 minutes
            });

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                PrefixResolver = PrefixResolverAsync, // Set the command prefix that will be used by the bot
                EnableDms = false, // Set the boolean for responding to direct messages
                //EnableDefaultHelp = false,
                EnableMentionPrefix = true, // Set the boolean for mentioning the bot as a command prefix
                CaseSensitive = false,
                //DefaultHelpChecks = new List<CheckBaseAttribute>()
            });
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;
            Commands.SetHelpFormatter<HelpFormatter>();
            Commands.RegisterCommands<BotModule>();
            Commands.RegisterCommands<OwnerModule>();

            Commands.RegisterCommands<OverwatchModule>();
            Commands.RegisterCommands<PokemonModule>();
            Commands.RegisterCommands<SmashModule>();
            Commands.RegisterCommands<TeamFortressModule>();

            Commands.RegisterCommands<MathModule>();
            Commands.RegisterCommands<MiscModule>();

            Commands.RegisterCommands<DictionaryModule>();
            Commands.RegisterCommands<GoogleModule>();
            Commands.RegisterCommands<IMDBModule>();
            Commands.RegisterCommands<ImgurModule>();
            Commands.RegisterCommands<SimpsonsModule>();
            Commands.RegisterCommands<SteamModule>();
            Commands.RegisterCommands<TwitchModule>();
            Commands.RegisterCommands<WikipediaModule>();
            Commands.RegisterCommands<YouTubeModule>();

            Commands.RegisterCommands<ChannelModule>();
            Commands.RegisterCommands<MessagesModule>();
            Commands.RegisterCommands<PollModule>();
            Commands.RegisterCommands<ServerModule>();
            Commands.RegisterCommands<UserModule>();
            Commands.RegisterCommands<UserRolesModule>();

            // Start the uptime counter
            Console.Title = GlobalVariables.Name + " (" + GlobalVariables.Version + ")";
            GlobalVariables.ProcessStarted = DateTime.Now;
            await BotServices.UpdateSteamAsync().ConfigureAwait(false); // Update the Steam App list
            await PokemonService.GetPokemonDataAsync().ConfigureAwait(false); // Update the Pokemon list
            await Client.ConnectAsync(); // Connect and log into Discord
            await Task.Delay(-1).ConfigureAwait(false); // Prevent the console window from closing
        }

        private static Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "FlawBOT", GlobalVariables.Name + $", version: " + GlobalVariables.Version, DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task Client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "FlawBOT", $"Exception occured: " + e.Exception.GetType() + ": " + e.Exception.Message, DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "FlawBOT", $"'{e.Command.QualifiedName}' executed by {e.Context.User.Username} from {e.Context.Guild.Name} : {e.Context.Channel.Name}", DateTime.Now);
            return Task.CompletedTask;
        }

        private static async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            switch (e.Exception)
            {
                case CommandNotFoundException _:
                    //await BotServices.SendEmbedAsync(e.Context, ":no_entry: This command does not exist!", EmbedType.Error);
                    break;

                case NullReferenceException _:
                    //await BotServices.SendEmbedAsync(e.Context, ":mag: Data not found!", EmbedType.Error);
                    break;

                case ArgumentNullException _:
                    await BotServices.SendEmbedAsync(e.Context, ":no_entry: Not enough arguments supplied to the command!", EmbedType.Error);
                    break;

                case ArgumentException _:
                    if (e.Exception.Message.Contains("Not enough arguments supplied to the command"))
                        await BotServices.SendEmbedAsync(e.Context, ":no_entry: Not enough arguments supplied to the command!", EmbedType.Error);
                    break;

                case InvalidDataException _:
                    if (e.Exception.Message.Contains("The data within the stream was not valid image data"))
                        await BotServices.SendEmbedAsync(e.Context, ":no_entry: Provided URL is not an image type!", EmbedType.Error);
                    break;

                default:
                    if (e.Exception.Message.Contains("Given emote was not found"))
                        await BotServices.SendEmbedAsync(e.Context, ":no_entry: Suggested emote was not found!", EmbedType.Error);
                    if (e.Exception.Message.Contains("Unauthorized: 403"))
                        await BotServices.SendEmbedAsync(e.Context, ":no_entry: Unsufficient Permissions", EmbedType.Error);
                    else
                        e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "FlawBOT", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now); // DEBUG ONLY
                    break;
            }
        }

        private static Task<int> PrefixResolverAsync(DiscordMessage m)
        {
            return Task.FromResult(m.GetStringPrefixLength(GlobalVariables.config.CommandPrefix));
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

                    default:
                        ccfg = ConsoleColor.Gray;
                        ccbg = ConsoleColor.Black;
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