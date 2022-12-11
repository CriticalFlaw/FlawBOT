using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Services;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Bot
{
    public class BotModule : ApplicationCommandModule
    {
        #region COMMAND_INFO

        [SlashCommand("info", "Retrieve information about FlawBOT.")]
        public async Task BotInfo(InteractionContext ctx)
        {
            var settings = Program.Settings;
            var uptime = DateTime.Now - settings.ProcessStarted;
            var days = uptime.Days > 0 ? $"({uptime.Days:00} days)" : string.Empty;

            // Build the output.
            var output = new DiscordEmbedBuilder()
                .WithTitle(settings.Name)
                .WithDescription("A multipurpose Discord bot written in C# with [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus/).")
                .AddField(":clock1: Uptime", $"{uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00} {days}", true)
                .AddField(":link: Links", $"[Commands]({settings.DocsLink}) **|** [GitHub]({settings.GitHubLink})", true)
                .WithFooter($"Thank you for using {settings.Name} (v{settings.Version})")
                .WithUrl(settings.GitHubLink)
                .WithColor(settings.DefaultColor);

            // Send the output.
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_INFO

        #region COMMAND_PING

        [SlashCommand("ping", "Ping the FlawBOT client.")]
        public async Task PingBot(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $":ping_pong: Pong! Ping: **{ctx.Client.Ping}**ms").ConfigureAwait(false);
        }

        #endregion COMMAND_PING

        #region COMMAND_UPTIME

        [SlashCommand("uptime", "Retrieve the FlawBOT uptime.")]
        public async Task Uptime(InteractionContext ctx)
        {
            var settings = Program.Settings;
            var uptime = DateTime.Now - settings.ProcessStarted;
            var days = uptime.Days > 0 ? $"({uptime.Days:00} days)" : string.Empty;
            await BotServices.SendResponseAsync(ctx, $":clock1: {settings.Name} has been online for {uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds} {days}").ConfigureAwait(false);
        }

        #endregion COMMAND_UPTIME
    }
}