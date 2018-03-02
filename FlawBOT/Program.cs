using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using FlawBOT.Modules;
using FlawBOT.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
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
            string JSON = null; // Load the configuration file
            using (var FRD = File.OpenRead("config.json"))
            using (var SRD = new StreamReader(FRD, new UTF8Encoding(false)))
                JSON = await SRD.ReadToEndAsync();

            var CFG = new DiscordConfiguration()
            {
                Token = JsonConvert.DeserializeObject<APITokenService.APITokenList>(JSON).Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = true
            };

            var CMD = new CommandsNextConfiguration
            {
                StringPrefix = JsonConvert.DeserializeObject<APITokenService.APITokenList>(JSON).CommandPrefix,
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
            Commands.SetHelpFormatter<HelperService>();         // Set up the custom help formatter
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;
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
            // Update the Steam and TF2 lists
            Client.DebugLogger.LogMessage(LogLevel.Info, "FlawBOT", "Updating Steam database...", DateTime.Now);
            GlobalVariables.UpdateSteamAsync();
            Client.DebugLogger.LogMessage(LogLevel.Info, "FlawBOT", "Ready!", DateTime.Now);
            await Client.ConnectAsync();                        // Connect and log into Discord
            await Task.Delay(-1);                               // Prevent the console window from closing
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "FlawBOT", "FlawBOT, version: 0.9.0 (Build 20180228)", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "FlawBOT", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "FlawBOT", $"'{e.Command.QualifiedName}' successfully executed by {e.Context.User.Username} from {e.Context.Guild.Name} : {e.Context.Channel.Name}", DateTime.Now);
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "FlawBOT", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);
            if (e.Exception is ChecksFailedException)
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Access Denied")
                    .WithDescription(":no_entry: Insufficient permissions.")
                    .WithColor(DiscordColor.Red);
                await e.Context.RespondAsync(embed: output.Build());
            }
            else if (e.Exception is CommandNotFoundException)
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
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Invalid parameters")
                    .WithDescription(":no_entry: Parameter provide is invalid or does not exist.")
                    .WithColor(DiscordColor.Red);
                await e.Context.RespondAsync(embed: output.Build());
            }
        }
    }
}