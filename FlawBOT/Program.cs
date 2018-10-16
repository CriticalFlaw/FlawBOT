using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using FlawBOT.Modules;
using FlawBOT.Services;
using SteamWebAPI2.Interfaces;
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
            var cfg = new DiscordConfiguration
            {
                Token = service.GetAPIToken("discord"),
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = false,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                LargeThreshold = 250
            };

            var cmd = new CommandsNextConfiguration
            {
                PrefixResolver = PrefixResolverAsync, // Set the command prefix that will be used by the bot
                EnableDms = false, // Set the boolean for responding to direct messages
                EnableDefaultHelp = false,
                EnableMentionPrefix = true, // Set the boolean for mentioning the bot as a command prefix
                CaseSensitive = false,
                DefaultHelpChecks = new List<CheckBaseAttribute>()
            };

            Client = new DiscordClient(cfg);
            Client.Ready += Client_Ready;
            Client.ClientErrored += Client_ClientError;
            Client.DebugLogger.LogMessageReceived += Client_LogMessageHandler;
            Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehavior = TimeoutBehaviour.Ignore, // Default pagination behaviour to just ignore the reactions
                PaginationTimeout = TimeSpan.FromMinutes(5), // Default pagination timeout to 5 minutes
                Timeout = TimeSpan.FromMinutes(2) // Default timeout for other actions to 2 minutes
            });
            Commands = Client.UseCommandsNext(cmd);
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;
            Commands.SetHelpFormatter<HelperService>(); // Set up the custom help formatter
            Commands.RegisterCommands<BotModule>();
            Commands.RegisterCommands<CommonModule>();
            Commands.RegisterCommands<GoogleModule>();
            Commands.RegisterCommands<ModeratorModule>();
            Commands.RegisterCommands<ServerModule>();
            Commands.RegisterCommands<SteamModule>();

            // Start the uptime counter
            GlobalVariables.ProcessStarted = DateTime.Now;
            await UpdateSteamAsync().ConfigureAwait(false); // Update the Steam App list
            await Client.ConnectAsync(); // Connect and log into Discord
            await Task.Delay(-1).ConfigureAwait(false); // Prevent the console window from closing
        }

        private static Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "FlawBOT", $"{GlobalVariables.Name}, version: {GlobalVariables.Version}", DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task Client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "FlawBOT", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
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
                    break;

                case ArgumentNullException _:
                    await e.Context.RespondAsync(":no_entry: Not enough arguments supplied to the command!");
                    break;

                case ArgumentException _:
                    if (e.Exception.Message.Contains("Not enough arguments supplied to the command"))
                        await e.Context.RespondAsync(":no_entry: Not enough arguments supplied to the command!");
                    break;

                case InvalidDataException _:
                    if (e.Exception.Message.Contains("The data within the stream was not valid image data"))
                        await e.Context.RespondAsync(":no_entry: Provided URL is not an image type!");
                    break;

                default:
                    if (e.Exception.Message.Contains("Given emote was not found"))
                        await e.Context.RespondAsync(":no_entry: Given emote was not found!");
                    else
                        e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "FlawBOT", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now); // DEBUG ONLY
                    break;
            }
        }

        private static Task<int> PrefixResolverAsync(DiscordMessage m)
        {
            var service = new BotServices();
            return Task.FromResult(m.GetStringPrefixLength(service.GetAPIToken("prefix")));
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

        private static Task UpdateSteamAsync()
        {
            Console.WriteLine("Updating Steam App List...");
            var service = new BotServices();
            var token = service.GetAPIToken("steam");
            var apps = new SteamApps(token);
            var games = apps.GetAppListAsync().Result.Data;
            GlobalVariables.SteamAppList.Clear();
            foreach (var game in games)
                if (!string.IsNullOrWhiteSpace(game.Name))
                    GlobalVariables.SteamAppList.Add(Convert.ToUInt32(game.AppId), game.Name);

            Console.WriteLine("Updating TF2 Item Schema...");
            var schema = new EconItems(token, EconItemsAppId.TeamFortress2);
            var items = schema.GetSchemaForTF2Async();
            GlobalVariables.TFItemSchema.Clear();
            //foreach (var item in items.Result.Data.Items)
            //    if (!string.IsNullOrWhiteSpace(item.ItemName))
            //        GlobalVariables.TFItemSchema.Add(Convert.ToUInt32(item.DefIndex), item.ItemName);

            return Task.CompletedTask;
        }
    }
}