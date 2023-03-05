using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Services;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("bot", "Slash command group for modal bot commands.")]
    public class BotModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns basic information about FlawBOT.
        /// </summary>
        [SlashCommand("info", "Returns basic information about FlawBOT.")]
        public async Task GetBotInfo(InteractionContext ctx)
        {
            var settings = Program.Settings;
            var output = new DiscordEmbedBuilder()
                .WithTitle(settings.Name)
                .WithDescription($"A multipurpose Discord bot written in C# with DSharpPlus.")
                .AddField(":clock1: Uptime", BotServices.GetCurrentUptime(), true)
                .AddField(":link: Links", $"[Commands]({settings.DocsLink}) **|** [GitHub]({settings.GitHubLink})", true)
                .WithFooter($"Thank you for using {settings.Name} (v{settings.Version})")
                .WithUrl(settings.GitHubLink)
                .WithColor(settings.DefaultColor);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the current FlawBOT ping rate.
        /// </summary>
        [SlashCommand("ping", "Returns the current FlawBOT ping rate.")]
        public async Task GetBotPing(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $":ping_pong: Pong! Ping: {Formatter.Bold(ctx.Client.Ping.ToString())}ms").ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the current FlawBOT uptime.
        /// </summary>
        [SlashCommand("uptime", "Returns the current FlawBOT uptime.")]
        public async Task GetBotUptime(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, ":clock1: " + BotServices.GetCurrentUptime()).ConfigureAwait(false);
        }

        /// <summary>
        /// Changes FlawBOT's activity.
        /// </summary>
        [RequireOwner]
        [SlashCommand("activity", "Changes FlawBOT's activity.")]
        public async Task SetBotActivity(InteractionContext ctx, [Option("activity", "Activity name.")] string activity)
        {
            if (string.IsNullOrWhiteSpace(activity))
            {
                await ctx.Client.UpdateStatusAsync().ConfigureAwait(false);
                return;
            }

            var game = new DiscordActivity(activity);
            await ctx.Client.UpdateStatusAsync(game).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{Program.Settings.Name} activity has been changed to Playing {Formatter.Bold(game.Name)}").ConfigureAwait(false);
        }

        /// <summary>
        /// Changes FlawBOT's avatar.
        /// </summary>
        [RequireOwner]
        [SlashCommand("avatar", "Changes FlawBOT's avatar.")]
        public async Task SetBotAvatar(InteractionContext ctx, [Option("url", "Image URL in JPG, PNG or IMG format.")] string url)
        {
            var stream = BotServices.CheckImageInput(ctx, url).Result;
            if (stream is null) return;
            await ctx.Client.UpdateCurrentUserAsync(avatar: stream).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{Program.Settings.Name} avatar has been updated!").ConfigureAwait(false);
        }

        /// <summary>
        /// Changes FlawBOT's status.
        /// </summary>
        [RequireOwner]
        [SlashCommand("status", "Changes FlawBOT's status.")]
        public async Task SetBotStatus(InteractionContext ctx, [Option("status", "Discord Status: Online, Idle, DND or Offline.")] string status = "ONLINE")
        {
            switch (status.Trim().ToUpperInvariant())
            {
                case "OFF":
                case "OFFLINE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Offline).ConfigureAwait(false);
                    await ctx.CreateResponseAsync($"{Program.Settings.Name} status has been changed to Offline").ConfigureAwait(false);
                    break;

                case "INVISIBLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Invisible).ConfigureAwait(false);
                    await ctx.CreateResponseAsync($"{Program.Settings.Name} status has been changed to Invisible").ConfigureAwait(false);
                    break;

                case "IDLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Idle).ConfigureAwait(false);
                    await ctx.CreateResponseAsync($"{Program.Settings.Name} status has been changed to Idle").ConfigureAwait(false);
                    break;

                case "DND":
                case "DO NOT DISTURB":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.DoNotDisturb).ConfigureAwait(false);
                    await ctx.CreateResponseAsync($"{Program.Settings.Name} status has been changed to Do Not Disturb").ConfigureAwait(false);
                    break;

                default:
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Online).ConfigureAwait(false);
                    await ctx.CreateResponseAsync($"{Program.Settings.Name} status has been changed to Online").ConfigureAwait(false);
                    break;
            }
        }

        /// <summary>
        /// Changes FlawBOT's nickname.
        /// </summary>
        [RequireOwner]
        [SlashCommand("nickname", "Set FlawBOT's nickname.")]
        public async Task SetBotUsername(InteractionContext ctx, [Option("nickname", "New nickname for FlawBOT.")] string nickname)
        {
            var oldName = ctx.Client.CurrentUser.Username;
            var newName = string.IsNullOrWhiteSpace(nickname) ? Program.Settings.Name : nickname;
            await ctx.Client.UpdateCurrentUserAsync(newName).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{oldName}'s username has been changed to {newName}").ConfigureAwait(false);
        }
    }
}