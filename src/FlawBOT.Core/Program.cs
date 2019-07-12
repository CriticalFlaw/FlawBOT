using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using FlawBOT.Common;
using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using FlawBOT.Modules;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FlawBOT
{
    public class Program
    {
        public DiscordClient Client { get; set; }
        public CommandsNextExtension Commands { get; private set; }
        private static readonly object _lock = new object();

        public static async Task Main(string[] args)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                Program app = new Program();
                app.RunBotAsync().GetAwaiter().GetResult();
                await Task.Delay(Timeout.Infinite).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException occured: {e.GetType()} :\n{e.Message}");
                if (!(e.InnerException is null))
                    Console.WriteLine($"Inner exception: {e.InnerException.GetType()} :\n{e.InnerException.Message}");
                Console.ReadKey();
            }
            Console.WriteLine("\nPowering off...");
        }

        public async Task RunBotAsync()
        {
            var service = new BotServices();
            service.UpdateTokenList();

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
                PaginationBehaviour = PaginationBehaviour.Ignore, // Default pagination behaviour to just ignore the reactions
                Timeout = TimeSpan.FromMinutes(2) // Default pagination timeout to 2 minutes
            });

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                PrefixResolver = PrefixResolverAsync, // Set the command prefix that will be used by the bot
                EnableDms = false, // Set the boolean for responding to direct messages
                EnableMentionPrefix = true, // Set the boolean for mentioning the bot as a command prefix
                CaseSensitive = false,
            });
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;
            Commands.SetHelpFormatter<HelpFormatter>();
            Commands.RegisterCommands<BotModule>();
            Commands.RegisterCommands<OwnerModule>();
            Commands.RegisterCommands<PokemonModule>();
            Commands.RegisterCommands<SmashModule>();
            Commands.RegisterCommands<SpeedrunModule>();
            Commands.RegisterCommands<TeamFortressModule>();
            Commands.RegisterCommands<MathModule>();
            Commands.RegisterCommands<MiscModule>();
            Commands.RegisterCommands<PollModule>();
            Commands.RegisterCommands<DictionaryModule>();
            Commands.RegisterCommands<GoogleModule>();
            Commands.RegisterCommands<ImgurModule>();
            Commands.RegisterCommands<OMDBModule>();
            Commands.RegisterCommands<RedditModule>();
            Commands.RegisterCommands<SimpsonsModule>();
            Commands.RegisterCommands<SteamModule>();
            Commands.RegisterCommands<TwitchModule>();
            Commands.RegisterCommands<WikipediaModule>();
            Commands.RegisterCommands<YouTubeModule>();
            Commands.RegisterCommands<ChannelModule>();
            Commands.RegisterCommands<RoleModule>();
            Commands.RegisterCommands<ServerModule>();
            Commands.RegisterCommands<UserModule>();

            // Start the uptime counter
            Console.Title = SharedData.Name + " (" + SharedData.Version + ")";
            SharedData.ProcessStarted = DateTime.Now;
            await SteamService.UpdateSteamListAsync().ConfigureAwait(false);
            await TeamFortressService.UpdateTF2SchemaAsync().ConfigureAwait(false);
            await PokemonService.UpdatePokemonListAsync().ConfigureAwait(false);
            await Client.ConnectAsync(); // Connect and log into Discord
            await Task.Delay(-1).ConfigureAwait(false); // Prevent the console window from closing
        }

        private static Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, SharedData.Name, SharedData.Name + ", version: " + SharedData.Version, DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task Client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, SharedData.Name, "Exception occured: " + e.Exception.GetType() + ": " + e.Exception.Message, DateTime.Now);
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
                case CommandNotFoundException _:
                    //await BotServices.SendEmbedAsync(e.Context, "This command does not exist!", EmbedType.Error);
                    break;

                case NullReferenceException _:
                    //await BotServices.SendEmbedAsync(e.Context, "Data not found!", EmbedType.Missing);
                    break;

                case ArgumentNullException _:
                    await BotServices.SendEmbedAsync(e.Context, "Not enough arguments supplied to the command!", EmbedType.Error);
                    break;

                case ArgumentException _:
                    if (e.Exception.Message.Contains("Not enough arguments supplied to the command"))
                        await BotServices.SendEmbedAsync(e.Context, "Not enough arguments supplied to the command!", EmbedType.Error);
                    break;

                case InvalidDataException _:
                    if (e.Exception.Message.Contains("The data within the stream was not valid image data"))
                        await BotServices.SendEmbedAsync(e.Context, "Provided URL is not an image type!", EmbedType.Error);
                    break;

                default:
                    if (e.Exception.Message.Contains("Given emote was not found"))
                        await BotServices.SendEmbedAsync(e.Context, "Suggested emote was not found!", EmbedType.Error);
                    if (e.Exception.Message.Contains("Unauthorized: 403"))
                        await BotServices.SendEmbedAsync(e.Context, "Unsufficient Permissions", EmbedType.Error);
                    else
                        e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, SharedData.Name, $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now); // DEBUG ONLY
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