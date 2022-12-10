using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Properties;
using FlawBOT.Services;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Bot
{
    [Group("bot")]
    [Aliases("flaw")]
    [Description("Basic commands for interacting with FlawBOT.")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class BotModule : BaseCommandModule
    {
        #region COMMAND_INFO

        [Command("info")]
        [Aliases("about")]
        [Description("Retrieve information about FlawBOT.")]
        public async Task BotInfo(CommandContext ctx)
        {
            var settings = Program.Settings;
            var uptime = DateTime.Now - settings.ProcessStarted;
            var output = new DiscordEmbedBuilder()
                .WithTitle(settings.Name)
                .WithDescription(
                    "A multipurpose Discord bot written in C# with [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus/).")
                .AddField(":clock1: Uptime",
                    $"{(int)uptime.TotalDays:00} days {uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}", true)
                .AddField(":link: Links",
                    $"[Commands]({settings.DocsLink}) **|** [GitHub]({settings.GitHubLink})", true)
                .WithFooter($"Thank you for using {settings.Name} (v{settings.Version})")
                .WithUrl(settings.GitHubLink)
                .WithColor(settings.DefaultColor);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_INFO

        #region COMMAND_LEAVE

        [Command("leave")]
        [Description("Make FlawBOT leave the server.")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task LeaveServer(CommandContext ctx)
        {
            var settings = Program.Settings;
            await ctx.RespondAsync($"Are you sure you want {settings.Name} to leave the server?")
                .ConfigureAwait(false);
            var message = await ctx
                .RespondAsync(Resources.INFO_RESPOND)
                .ConfigureAwait(false);
            var interactivity = await BotServices.GetUserInteractivity(ctx, "yes", 10).ConfigureAwait(false);
            if (interactivity.Result is null)
            {
                await message.ModifyAsync($"~~{message.Content}~~ {Resources.INFO_REQ_TIMEOUT}")
                    .ConfigureAwait(false);
                return;
            }

            await BotServices.SendResponseAsync(ctx, $"Thank you for using {settings.Name}").ConfigureAwait(false);
            await ctx.Guild.LeaveAsync().ConfigureAwait(false);
        }

        #endregion COMMAND_LEAVE

        #region COMMAND_PING

        [Command("ping")]
        [Aliases("pong")]
        [Description("Ping the FlawBOT client.")]
        public async Task PingBot(CommandContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $":ping_pong: Pong! Ping: **{ctx.Client.Ping}**ms")
                .ConfigureAwait(false);
        }

        #endregion COMMAND_PING

        #region COMMAND_REPORT

        [Hidden]
        [Command("report")]
        [Aliases("issue")]
        [Description("Report a problem with FlawBOT to the developer.")]
        public async Task ReportIssue(CommandContext ctx,
            [Description("Detailed description of the issue.")] [RemainingText]
            string report)
        {
            if (string.IsNullOrWhiteSpace(report) || report.Length < 50)
            {
                await ctx.RespondAsync(Resources.ERR_REPORT_LENGTH).ConfigureAwait(false);
                return;
            }

            await ctx.RespondAsync(Resources.INFO_REPORT_SENDER).ConfigureAwait(false);
            var message = await ctx
                .RespondAsync(Resources.INFO_RESPOND)
                .ConfigureAwait(false);
            var interactivity = await BotServices.GetUserInteractivity(ctx, "yes", 10).ConfigureAwait(false);
            if (interactivity.Result is null)
            {
                await message.ModifyAsync($"~~{message.Content}~~ {Resources.INFO_REQ_TIMEOUT}")
                    .ConfigureAwait(false);
            }
            else
            {
                var settings = Program.Settings;
                var dm = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);
                var output = new DiscordEmbedBuilder()
                    .WithAuthor(ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator,
                        iconUrl: ctx.User.AvatarUrl ?? ctx.User.DefaultAvatarUrl)
                    .AddField("Issue", report)
                    .AddField("Sent By", ctx.User.Username + "#" + ctx.User.Discriminator)
                    .AddField("Server", ctx.Guild.Name + $" (ID: {ctx.Guild.Id})")
                    .AddField("Owner", ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator)
                    .AddField("Confirm",
                        $"[Click here to add this issue to GitHub]({settings.GitHubLink}/issues/new)")
                    .WithColor(settings.DefaultColor);
                await dm.SendMessageAsync(output.Build()).ConfigureAwait(false);
                await ctx.RespondAsync("Thank You! Your report has been submitted.").ConfigureAwait(false);
            }
        }

        #endregion COMMAND_REPORT

        #region COMMAND_UPTIME

        [Command("uptime")]
        [Description("Retrieve the FlawBOT uptime")]
        public async Task Uptime(CommandContext ctx)
        {
            var settings = Program.Settings;
            var uptime = DateTime.Now - settings.ProcessStarted;
            var days = uptime.Days > 0 ? $"({uptime.Days:00} days)" : string.Empty;
            await BotServices.SendResponseAsync(ctx,
                    $":clock1: {settings.Name} has been online for {uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds} {days}")
                .ConfigureAwait(false);
        }

        #endregion COMMAND_UPTIME

        #region OWNERS-ONLY

        #region COMMAND_ACTIVITY

        [RequireOwner]
        [Hidden]
        [Command("activity")]
        [Aliases("setactivity")]
        [Description("Set FlawBOT's activity.")]
        public async Task SetBotActivity(CommandContext ctx,
            [Description("Name of the activity.")] [RemainingText]
            string activity)
        {
            if (string.IsNullOrWhiteSpace(activity))
            {
                await ctx.Client.UpdateStatusAsync().ConfigureAwait(false);
                return;
            }

            var game = new DiscordActivity(activity);
            await ctx.Client.UpdateStatusAsync(game).ConfigureAwait(false);
            await ctx.RespondAsync($"{Program.Settings.Name} activity has been changed to Playing {game.Name}")
                .ConfigureAwait(false);
        }

        #endregion COMMAND_ACTIVITY

        #region COMMAND_AVATAR

        [RequireOwner]
        [Hidden]
        [Command("avatar")]
        [Aliases("setavatar", "pfp", "photo")]
        [Description("Set FlawBOT's avatar.")]
        public async Task SetBotAvatar(CommandContext ctx,
            [Description("Image URL. Must be in JPG, PNG or IMG format.")]
            string query)
        {
            var stream = BotServices.CheckImageInput(ctx, query).Result;
            if (stream.Length <= 0) return;
            await ctx.Client.UpdateCurrentUserAsync(avatar: stream).ConfigureAwait(false);
            await ctx.RespondAsync($"{Program.Settings.Name} avatar has been updated!").ConfigureAwait(false);
        }

        #endregion COMMAND_AVATAR

        #region COMMAND_STATUS

        [RequireOwner]
        [Hidden]
        [Command("status")]
        [Aliases("setstatus", "state")]
        [Description("Set FlawBOT's status.")]
        public async Task SetBotStatus(CommandContext ctx,
            [Description("Activity Status. Online, Idle, DND or Offline")] [RemainingText]
            string status)
        {
            var settings = Program.Settings;
            status ??= "ONLINE";
            switch (status.Trim().ToUpperInvariant())
            {
                case "OFF":
                case "OFFLINE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Offline).ConfigureAwait(false);
                    await BotServices.SendResponseAsync(ctx, $"{settings.Name} status has been changed to Offline")
                        .ConfigureAwait(false);
                    break;

                case "INVISIBLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Invisible).ConfigureAwait(false);
                    await BotServices.SendResponseAsync(ctx, $"{settings.Name} status has been changed to Invisible")
                        .ConfigureAwait(false);
                    break;

                case "IDLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Idle).ConfigureAwait(false);
                    await BotServices.SendResponseAsync(ctx, $"{settings.Name} status has been changed to Idle")
                        .ConfigureAwait(false);
                    break;

                case "DND":
                case "DO NOT DISTURB":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.DoNotDisturb).ConfigureAwait(false);
                    await BotServices
                        .SendResponseAsync(ctx, $"{settings.Name} status has been changed to Do Not Disturb")
                        .ConfigureAwait(false);
                    break;

                default:
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Online).ConfigureAwait(false);
                    await BotServices.SendResponseAsync(ctx, $"{settings.Name} status has been changed to Online")
                        .ConfigureAwait(false);
                    break;
            }
        }

        #endregion COMMAND_STATUS

        #region COMMAND_USERNAME

        [RequireOwner]
        [Hidden]
        [Command("username")]
        [Aliases("setusername", "name", "setname", "nickname", "nick")]
        [Description("Set FlawBOT's username.")]
        public async Task SetBotUsername(CommandContext ctx,
            [Description("New nickname for FlawBOT.")] [RemainingText]
            string name)
        {
            var oldName = ctx.Client.CurrentUser.Username;
            var newName = string.IsNullOrWhiteSpace(name) ? Program.Settings.Name : name;
            await ctx.Client.UpdateCurrentUserAsync(newName).ConfigureAwait(false);
            await BotServices.SendResponseAsync(ctx, $"{oldName}'s username has been changed to {newName}")
                .ConfigureAwait(false);
        }

        #endregion COMMAND_USERNAME

        #endregion OWNERS-ONLY
    }
}