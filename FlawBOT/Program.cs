using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using FlawBOT.Modules;
using FlawBOT.Services;
using SteamWebAPI2.Interfaces;

namespace FlawBOT
{
    public class Program
    {
        public DiscordClient Client { get; set; }
        public CommandsNextModule Commands { get; set; }

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
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = true
            };

            var cmd = new CommandsNextConfiguration
            {
                StringPrefix = service.GetAPIToken("prefix"), // Set the command prefix that will be used by the bot
                EnableDms = false, // Set the boolean for responding to direct messages
                EnableDefaultHelp = false,
                EnableMentionPrefix = true // Set the boolean for mentioning the bot as a command prefix
            };

            Client = new DiscordClient(cfg);
            Client.Ready += Client_Ready;
            Client.ClientErrored += Client_ClientError;
            Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = TimeoutBehaviour.Ignore, // Default pagination behaviour to just ignore the reactions
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

            // Set up the custom name and type converter
            var math = new MathService();
            CommandsNextUtilities.RegisterConverter(math);
            CommandsNextUtilities.RegisterUserFriendlyTypeName<MathService>("operation");
            GlobalVariables.ProcessStarted = DateTime.Now; // Start the uptime counter
            Client.DebugLogger.LogMessage(LogLevel.Info, "FlawBOT", "Updating Steam database...", DateTime.Now); // REMOVE
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
    }
}
