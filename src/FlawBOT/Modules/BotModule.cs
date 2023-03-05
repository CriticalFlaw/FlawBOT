using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("bot", "Slash command group for FlawBOT-specific commands.")]
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

        /// <summary>
        /// Makes FlawBOT leave the server.
        /// </summary>
        [SlashCommand("leave", "Make FlawBOT leave the server.")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task LeaveServer(CommandContext ctx)
        {
            await ctx.RespondAsync($"Are you sure you want {Program.Settings.Name} to leave the server?").ConfigureAwait(false);
            var message = await ctx.RespondAsync(Resources.INFO_RESPOND).ConfigureAwait(false);
            var interactivity = await BotServices.GetUserInteractivity(ctx, "yes", 10).ConfigureAwait(false);
            if (interactivity.Result is null)
            {
                await message.ModifyAsync($"~~{message.Content}~~ {Resources.INFO_REQ_TIMEOUT}").ConfigureAwait(false);
                return;
            }

            await BotServices.SendResponseAsync(ctx, $"Thank you for using {Program.Settings.Name}").ConfigureAwait(false);
            await ctx.Guild.LeaveAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Reports an issue with FlawBOT to the developer.
        /// </summary>
        [Hidden]
        [SlashCommand("report", "Reports an issue with FlawBOT to the developer.")]
        public async Task ReportBotIssue(CommandContext ctx, [Option("report", "Detailed description of the issue.")] string report)
        {
            if (string.IsNullOrWhiteSpace(report) || report.Length < 50)
            {
                await ctx.RespondAsync(Resources.ERR_REPORT_LENGTH).ConfigureAwait(false);
                return;
            }

            await ctx.RespondAsync(Resources.INFO_REPORT_SENDER).ConfigureAwait(false);
            var message = await ctx.RespondAsync(Resources.INFO_RESPOND).ConfigureAwait(false);
            var interactivity = await BotServices.GetUserInteractivity(ctx, "yes", 10).ConfigureAwait(false);
            if (interactivity.Result is null)
            {
                await message.ModifyAsync($"~~{message.Content}~~ {Resources.INFO_REQ_TIMEOUT}").ConfigureAwait(false);
            }
            else
            {
                var settings = Program.Settings;
                var output = new DiscordEmbedBuilder()
                    .WithAuthor(ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator, iconUrl: ctx.User.AvatarUrl ?? ctx.User.DefaultAvatarUrl)
                    .AddField("Issue", report)
                    .AddField("Sent By", ctx.User.Username + "#" + ctx.User.Discriminator)
                    .AddField("Server", ctx.Guild.Name + $" (ID: {ctx.Guild.Id})")
                    .AddField("Owner", ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator)
                    .AddField("Confirm", $"[Click here to add this issue to GitHub]({settings.GitHubLink}/issues/new)")
                    .WithColor(settings.DefaultColor);
                var dm = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);
                await dm.SendMessageAsync(output.Build()).ConfigureAwait(false);
                await ctx.RespondAsync("Thank You! Your report has been submitted.").ConfigureAwait(false);
            }
        }
    }
}