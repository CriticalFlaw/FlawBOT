using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class BotModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns basic bot information.
        /// </summary>
        [SlashCommand("info", "Retrieve information about FlawBOT.")]
        public async Task BotInfo(InteractionContext ctx)
        {
            var settings = Program.Settings;
            var uptime = DateTime.Now - settings.ProcessStarted;
            var days = uptime.Days > 0 ? $"({uptime.Days:00} days)" : string.Empty;
            var output = new DiscordEmbedBuilder()
                .WithTitle(settings.Name)
                .WithDescription($"A multipurpose Discord bot written in C# with DSharpPlus.")
                .AddField(":clock1: Uptime", $"{uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00} {days}", true)
                .AddField(":link: Links", $"[Commands]({settings.DocsLink}) **|** [GitHub]({settings.GitHubLink})", true)
                .WithFooter($"Thank you for using {settings.Name} (v{settings.Version})")
                .WithUrl(settings.GitHubLink)
                .WithColor(settings.DefaultColor);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the current bot ping rate.
        /// </summary>
        [SlashCommand("ping", "Ping the FlawBOT client.")]
        public async Task PingBot(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $":ping_pong: Pong! Ping: {Formatter.Bold(ctx.Client.Ping.ToString())}ms").ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the current bot uptime.
        /// </summary>
        [SlashCommand("uptime", "Retrieve the FlawBOT uptime.")]
        public async Task Uptime(InteractionContext ctx)
        {
            var settings = Program.Settings;
            var uptime = DateTime.Now - settings.ProcessStarted;
            var days = uptime.Days > 0 ? $"({uptime.Days:00} days)" : string.Empty;
            await BotServices.SendResponseAsync(ctx, $":clock1: {settings.Name} has been online for {uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds} {days}").ConfigureAwait(false);
        }
    }
}