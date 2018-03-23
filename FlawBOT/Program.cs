using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using FlawBOT.Modules;
using FlawBOT.Services;
using Microsoft.Extensions.DependencyInjection;
using SteamWebAPI2.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Terradue.ServiceModel.Syndication;

namespace FlawBOT
{
    public class Program
    {
        public DiscordClient Client { get; set; }
        public CommandsNextExtension Commands { get; private set; }
        private static Timer FeedCheckTimer { get; set; }
        private static readonly object _lock = new object();

        public static void Main(string[] args)
        {
            var app = new Program();
            app.RunBotAsync().GetAwaiter().GetResult();
        }

        public async Task RunBotAsync()
        {
            var service = new APITokenService();
            var cfg = new DiscordConfiguration
            {
                Token = service.GetAPIToken("discord"),
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = false,  //true
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                LargeThreshold = 250
            };
            // initialize cnext dependencies
            var deps = new ServiceCollection().AddSingleton(Client);
            var cmd = new CommandsNextConfiguration
            {
                PrefixResolver = PrefixResolverAsync, // Set the command prefix that will be used by the bot
                EnableDms = false, // Set the boolean for responding to direct messages
                EnableDefaultHelp = true,   // false
                EnableMentionPrefix = true, // Set the boolean for mentioning the bot as a command prefix
                CaseSensitive = false,
                DefaultHelpChecks = new List<CheckBaseAttribute>(),
                //Services = deps.BuildServiceProvider()
            };

            Client = new DiscordClient(cfg);
            Client.Ready += Client_Ready;
            Client.ClientErrored += Client_ClientError;
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
            //Commands.RegisterCommands<BotModule>();
            //Commands.RegisterCommands<CommonModule>();
            //Commands.RegisterCommands<GoogleModule>();
            //Commands.RegisterCommands<ModeratorModule>();
            //Commands.RegisterCommands<ServerModule>();
            //Commands.RegisterCommands<SteamModule>();

            // Set up the custom name and type converter
            GlobalVariables.ProcessStarted = DateTime.Now; // Start the uptime counter
            //Client.MessageCreated += Client_OnMessageCreated;
            //Client.SocketErrored += Client_OnSocketErrored;
            Client.DebugLogger.LogMessageReceived += Client_LogMessageHandler;
            //Interactivity = this.Client.UseInteractivity(new InteractivityConfiguration());

            Commands.RegisterCommands(Assembly.GetExecutingAssembly());
            // Start the uptime counter
            Commands.SetHelpFormatter<HelperService>();
            GlobalVariables.ProcessStarted = DateTime.Now;
            Client.DebugLogger.LogMessage(LogLevel.Info, "FlawBOT", "Updating Steam database...", DateTime.Now); // REMOVE
            FeedCheckTimer = new Timer(FeedCheckTimerCallback, null, TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1));
            await UpdateSteamAsync(); // Update the Steam App list
            await Client.ConnectAsync(); // Connect and log into Discord
            await Task.Delay(-1); // Prevent the console window from closing
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
                    //await e.Context.RespondAsync(":no_entry: This command does not exist!");
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
            var service = new APITokenService();
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
            var service = new APITokenService();
            var token = service.GetAPIToken("steam");
            var apps = new SteamApps(token);
            var games = apps.GetAppListAsync().Result.Data;
            GlobalVariables.SteamAppList.Clear();
            foreach (var game in games)
                if (!string.IsNullOrWhiteSpace(game.Name))
                    GlobalVariables.SteamAppList.Add(Convert.ToUInt32(game.AppId), game.Name);
            return Task.CompletedTask;
            //var schema = new EconItems(token, EconItemsAppId.TeamFortress2);
            //var items = await schema.GetSchemaForTF2Async();
            //GlobalVariables.ItemSchema.Clear();
            //foreach (var item in items.Data.Items)
            //    if (!string.IsNullOrWhiteSpace(item.ItemName))
            //        GlobalVariables.ItemSchema.Add(Convert.ToInt32(item.DefIndex), item.ItemName);
        }

        private static void FeedCheckTimerCallback(object _)
        {
            var client = _ as DiscordClient;
            try
            {
                CheckFeedsForChangesAsync(client).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating RSS feeds! Exception: {ex.Message}");
            }
        }

        public static async Task CheckFeedsForChangesAsync(object _)
        {
            var client = _ as DiscordClient;
            var database = new DatabaseService();
            var feeds = await database.GetAllSubscriptionsAsync().ConfigureAwait(false);
            var regexes = new Regex("<span> *<a +href *= *\"([^\"]+)\"> *\\[link\\] *</a> *</span>", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            foreach (var feed in feeds)
            {
                try
                {
                    if (!feed.Subscriptions.Any())
                    {
                        await database.RemoveFeedAsync(feed.Id).ConfigureAwait(false);
                        continue;
                    }

                    var newest = DatabaseService.GetFeedResults(feed.URL).First();
                    var url = newest.Links.First().Uri.ToString();
                    if (string.Compare(url, feed.SavedURL, StringComparison.OrdinalIgnoreCase) == 0) continue;
                    await database.UpdateFeedSavedURLAsync(feed.Id, url).ConfigureAwait(false);
                    foreach (var sub in feed.Subscriptions)
                    {
                        DiscordChannel chn = null;
                        try
                        {
                            chn = await client.GetChannelAsync(sub.ChannelId).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error updating RSS feeds! Exception: {ex.Message}");
                        }
                        var output = new DiscordEmbedBuilder
                        {
                            Title = $"{newest.Title.Text}",
                            Url = url,
                            Timestamp = newest.LastUpdatedTime,
                            Color = DiscordColor.White
                        };

                        if (newest.Content is TextSyndicationContent content)
                        {
                            var matches = regexes.Match(content.Text);
                            if (matches.Success)
                                output.WithImageUrl(matches.Groups[1].Value);
                        }
                        if (!string.IsNullOrWhiteSpace(sub.QualifiedName))
                            output.AddField("From", sub.QualifiedName);
                        output.AddField("Link to content", url);
                        await chn.SendMessageAsync(embed: output.Build()).ConfigureAwait(false);
                        await Task.Delay(100).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating RSS feeds! Exception: {ex.Message}");
                }
            }
        }
    }
}