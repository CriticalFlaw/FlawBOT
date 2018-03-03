using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using FlawBOT.Modules;
using FlawBOT.Services;
using SteamWebAPI2.Interfaces;
using System;
using System.Threading.Tasks;

namespace FlawBOT
{
    public class Program
    {
        public DiscordClient Client { get; set; }
        public CommandsNextModule Commands { get; set; }

        public static void Main(string[] args)
        {
            var PROG = new Program();
            PROG.RunBotAsync().GetAwaiter().GetResult();
        }

        public async Task RunBotAsync()
        {
            APITokenService service = new APITokenService();
            var CFG = new DiscordConfiguration()
            {
                Token = service.GetAPIToken("discord"),
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = true
            };

            var CMD = new CommandsNextConfiguration
            {
                StringPrefix = service.GetAPIToken("prefix"),   // Set the command prefix that will be used by the bot
                EnableDms = false,                              // Set the boolean for responding to direct messages
                EnableMentionPrefix = true,                     // Set the boolean for mentioning the bot as a command prefix
            };

            Client = new DiscordClient(CFG);
            Client.Ready += Client_Ready;
            Client.ClientErrored += Client_ClientError;
            Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = TimeoutBehaviour.Ignore,  // Default pagination behaviour to just ignore the reactions
                PaginationTimeout = TimeSpan.FromMinutes(5),    // Default pagination timeout to 5 minutes
                Timeout = TimeSpan.FromMinutes(2)               // Default timeout for other actions to 2 minutes
            });
            Commands = Client.UseCommandsNext(CMD);
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;
            Commands.SetHelpFormatter<HelperService>();         // Set up the custom help formatter
            Commands.RegisterCommands<BotModule>();
            Commands.RegisterCommands<CommonModule>();
            Commands.RegisterCommands<GoogleModule>();
            Commands.RegisterCommands<ModeratorModule>();
            Commands.RegisterCommands<ServerModule>();
            Commands.RegisterCommands<SteamModule>();

            // Set up the custom name and type converter
            var MathCMD = new MathService();
            CommandsNextUtilities.RegisterConverter(MathCMD);
            CommandsNextUtilities.RegisterUserFriendlyTypeName<MathService>("operation");
            GlobalVariables.ProcessStarted = DateTime.Now;      // Start the uptime counter
            Client.DebugLogger.LogMessage(LogLevel.Info, "FlawBOT", "Updating Steam database...", DateTime.Now);    // REMOVE
            await UpdateSteamAsync();                           // Update the Steam and TF2 lists
            await Client.ConnectAsync();                        // Connect and log into Discord
            await Task.Delay(-1);                               // Prevent the console window from closing
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "FlawBOT", $"{GlobalVariables.Name}, version: {GlobalVariables.Version}", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "FlawBOT", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "FlawBOT", $"'{e.Command.QualifiedName}' executed by {e.Context.User.Username} from {e.Context.Guild.Name} : {e.Context.Channel.Name}", DateTime.Now);
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "FlawBOT", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);    // DEBUG ONLY
            if (e.Exception is CommandNotFoundException)
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Command Not Found")
                    .WithDescription(":no_entry: This command does not exist.")
                    .WithColor(DiscordColor.Red);
                await e.Context.RespondAsync(embed: output.Build());
            }
            else if (e.Exception is ArgumentNullException)
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Insufficient parameters")
                    .WithDescription(":no_entry: Not enough arguments supplied to the command.")
                    .WithColor(DiscordColor.Red);
                await e.Context.RespondAsync(embed: output.Build());
            }
            else if (e.Exception is ArgumentException)
            {
                if (e.Exception.Message.Contains("Not enough arguments supplied to the command"))
                    await e.Context.RespondAsync(":no_entry: Not enough arguments supplied to the command!");
            }
            else if (e.Exception is System.IO.InvalidDataException)
            {
                if (e.Exception.Message.Contains("The data within the stream was not valid image data"))
                    await e.Context.RespondAsync(":no_entry: Provided URL is not an image type!");
            }
            //else if (e.Exception is ChecksFailedException)
            //{
            //    var output = new DiscordEmbedBuilder()
            //        .WithTitle("Access Denied")
            //        .WithDescription(":no_entry: Insufficient permissions.")
            //        .WithColor(DiscordColor.Red);
            //    await e.Context.RespondAsync(embed: output.Build());
            //}
            else if (e.Exception.Message.Contains("Given emote was not found"))
                await e.Context.RespondAsync(":no_entry: Given emote was not found!");

        }

        private async Task UpdateSteamAsync()
        {
            // TF2 Item Schema
            APITokenService service = new APITokenService();
            string Token = service.GetAPIToken("steam");
            EconItems schema = new EconItems(Token, EconItemsAppId.TeamFortress2);
            var items = await schema.GetSchemaForTF2Async();
            GlobalVariables.TFItemSchema.Clear();
            foreach (var item in items.Data.Items)
                if (!string.IsNullOrWhiteSpace(item.ItemName))
                    GlobalVariables.TFItemSchema.Add(Convert.ToInt32(item.DefIndex), item.ItemName);

            // Steam App List
            SteamService steam = new SteamService();
            var games = await SteamService.GetSteamAppsListAsync();
            GlobalVariables.SteamAppList.Clear();
            foreach (var game in games.applist.apps)
                if (!string.IsNullOrWhiteSpace(game.name))
                    GlobalVariables.SteamAppList.Add(Convert.ToUInt32(game.appid), game.name);
        }
    }
}