using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Common;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Group("bot")]
    [Description("Basic commands for interacting with FlawBOT")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class BotModule : BaseCommandModule
    {
        #region COMMAND_INFO

        [Command("info")]
        [Aliases("i")]
        [Description("Retrieve FlawBOT information")]
        public async Task BotInfo(CommandContext ctx)
        {
            var uptime = DateTime.Now - SharedData.ProcessStarted;
            var output = new DiscordEmbedBuilder()
                .WithTitle(SharedData.Name)
                .WithDescription("A multipurpose Discord bot written in C# with [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus/).")
                .AddField(":clock1: Uptime", $"{(int)uptime.TotalDays:00} days {uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}", true)
                .AddField(":link: Links", $"[Commands]({SharedData.GitHubLink}wiki) **|** [Invite]({SharedData.InviteLink}) **|** [GitHub]({SharedData.GitHubLink})", true)
                .WithFooter("Thank you for using " + SharedData.Name + $" (v{SharedData.Version})")
                .WithUrl(SharedData.GitHubLink)
                .WithColor(SharedData.DefaultColor);
            await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_INFO

        #region COMMAND_LEAVE

        [Command("leave")]
        [Description("Make FlawBOT leave the current server")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task LeaveAsync(CommandContext ctx)
        {
            await ctx.RespondAsync($"Are you sure you want {SharedData.Name} to leave this server?");
            var message = await ctx.RespondAsync("Respond with **yes** to proceed or wait 10 seconds to cancel this operation.");
            var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Author.Id == ctx.User.Id && m.Content.ToLowerInvariant() == "yes", TimeSpan.FromSeconds(10));
            if (interactivity.Result is null)
                await message.ModifyAsync("~~" + message.Content + "~~ " + Resources.REQUEST_TIMEOUT);
            else
            {
                await BotServices.SendEmbedAsync(ctx, "Thank you for using " + SharedData.Name);
                await ctx.Guild.LeaveAsync();
            }
        }

        #endregion COMMAND_LEAVE

        #region COMMAND_PING

        [Command("ping")]
        [Aliases("pong")]
        [Description("Ping the FlawBOT client")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync($":ping_pong: Pong! Ping: **{ctx.Client.Ping}**ms");
        }

        #endregion COMMAND_PING

        #region COMMAND_REPORT

        [Command("report"), Hidden]
        [Aliases("issue")]
        [Description("Report a problem with FlawBOT to the developer. Please do not abuse.")]
        public async Task ReportIssue(CommandContext ctx,
            [Description("Detailed description of the issue")] [RemainingText] string report)
        {
            if (string.IsNullOrWhiteSpace(report) || report.Length < 50)
                await ctx.RespondAsync(Resources.ERR_REPORT_CHAR_LENGTH);
            else
            {
                await ctx.RespondAsync("The following information will be sent to the developer for investigation: User ID, Server ID, Server Name and Server Owner Name.");
                var message = await ctx.RespondAsync("Respond with **yes** to proceed or wait 10 seconds to cancel this operation.");
                var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Author.Id == ctx.User.Id && m.Content.ToLowerInvariant() == "yes", TimeSpan.FromSeconds(10));
                if (interactivity.Result is null)
                    await message.ModifyAsync("~~" + message.Content + "~~ " + Resources.REQUEST_TIMEOUT);
                else
                {
                    var dm = await ctx.Member.CreateDmChannelAsync();
                    var output = new DiscordEmbedBuilder()
                        .WithAuthor(ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator, iconUrl: ctx.User.AvatarUrl ?? ctx.User.DefaultAvatarUrl)
                        .AddField("Issue", report)
                        .AddField("Sent By", ctx.User.Username + "#" + ctx.User.Discriminator)
                        .AddField("Server", ctx.Guild.Name + $" (ID: {ctx.Guild.Id})")
                        .AddField("Owner", ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator)
                        .AddField("Confirm", $"[Click here to add this issue to GitHub]({SharedData.GitHubLink}/issues/new)")
                        .WithColor(SharedData.DefaultColor);
                    await dm.SendMessageAsync(embed: output.Build());
                    await BotServices.SendEmbedAsync(ctx, "Thank You! Your report has been submitted.", EmbedType.Good);
                }
            }
        }

        #endregion COMMAND_REPORT

        #region COMMAND_SAY

        [Command("say"), Hidden]
        [Aliases("echo")]
        [Description("Make FlawBOT repeat a message")]
        public Task Say(CommandContext ctx,
            [Description("Message for the bot to repeat")] [RemainingText] string message)
        {
            return ctx.RespondAsync((string.IsNullOrWhiteSpace(message)) ? ":thinking:" : message);
        }

        #endregion COMMAND_SAY

        #region COMMAND_UPTIME

        [Command("uptime")]
        [Description("Retrieve the FlawBOT uptime")]
        public async Task Uptime(CommandContext ctx)
        {
            var uptime = DateTime.Now - SharedData.ProcessStarted;
            var days = (uptime.Days > 0) ? $"({uptime.Days:00} days)" : null;
            await BotServices.SendEmbedAsync(ctx, $":clock1: {SharedData.Name} has been online for {uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds} {days}");
        }

        #endregion COMMAND_UPTIME

        #region OWNERS-ONLY

        #region COMMAND_ACTIVITY

        [RequireOwner]
        [Command("activity"), Hidden]
        [Aliases("setactivity")]
        [Description("Set FlawBOT's activity")]
        public async Task SetBotActivity(CommandContext ctx,
            [Description("Name of the activity")] [RemainingText] string activity)
        {
            if (string.IsNullOrWhiteSpace(activity))
            {
                await ctx.Client.UpdateStatusAsync(activity: null);
                await BotServices.SendEmbedAsync(ctx, SharedData.Name + " activity has been changed to Normal");
            }
            else
            {
                // TODO: Set the activity type
                var game = new DiscordActivity(activity);
                await ctx.Client.UpdateStatusAsync(activity: game);
                await BotServices.SendEmbedAsync(ctx, SharedData.Name + " activity has been changed to Playing " + game.Name, EmbedType.Good);
            }
        }

        #endregion COMMAND_ACTIVITY

        #region COMMAND_AVATAR

        [RequireOwner]
        [Command("avatar"), Hidden]
        [Aliases("setavatar")]
        [Description("Set FlawBOT's avatar")]
        public async Task SetBotAvatar(CommandContext ctx,
            [Description("Image URL. Must be in jpg, png or img format.")] string query)
        {
            var stream = BotServices.CheckImageInput(ctx, query).Result;
            if (stream.Length <= 0) return;
            await ctx.Client.UpdateCurrentUserAsync(avatar: stream);
            await BotServices.SendEmbedAsync(ctx, SharedData.Name + " avatar has been updated!", EmbedType.Good);
        }

        #endregion COMMAND_AVATAR

        #region COMMAND_STATUS

        [RequireOwner]
        [Command("status"), Hidden]
        [Aliases("setstatus")]
        [Description("Set FlawBOT's status")]
        public async Task SetBotStatus(CommandContext ctx,
            [Description("Activity Status. Online, Idle, DND or Offline")] [RemainingText] string status)
        {
            status = (string.IsNullOrWhiteSpace(status)) ? "ONLINE" : status;
            switch (status.Trim().ToUpperInvariant())
            {
                case "OFF":
                case "OFFLINE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Offline);
                    await BotServices.SendEmbedAsync(ctx, SharedData.Name + " status has been changed to Offline");
                    break;

                case "INVISIBLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Invisible);
                    await BotServices.SendEmbedAsync(ctx, SharedData.Name + " status has been changed to Invisible");
                    break;

                case "IDLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Idle);
                    await BotServices.SendEmbedAsync(ctx, SharedData.Name + " status has been changed to Idle");
                    break;

                case "DND":
                case "DO NOT DISTURB":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.DoNotDisturb);
                    await BotServices.SendEmbedAsync(ctx, SharedData.Name + " status has been changed to Do Not Disturb");
                    break;

                default:
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Online);
                    await BotServices.SendEmbedAsync(ctx, SharedData.Name + " status has been changed to Online");
                    break;
            }
        }

        #endregion COMMAND_STATUS

        #region COMMAND_UPDATE

        [RequireOwner]
        [Command("update"), Hidden]
        [Aliases("refresh")]
        [Description("Update FlawBOT libraries")]
        public async Task Update(CommandContext ctx)
        {
            var message = await ctx.RespondAsync("Starting update...");
            await SteamService.UpdateSteamListAsync().ConfigureAwait(false);
            await TeamFortressService.LoadTF2SchemaAsync().ConfigureAwait(false);
            await PokemonService.UpdatePokemonListAsync().ConfigureAwait(false);
            await message.ModifyAsync("Starting update...done!");
        }

        #endregion COMMAND_UPDATE

        #region COMMAND_USERNAME

        [RequireOwner]
        [Command("username"), Hidden]
        [Aliases("setusername", "name", "setname", "nickname")]
        [Description("Set FlawBOT's username")]
        public async Task SetBotUsername(CommandContext ctx,
            [Description("New bot username")] [RemainingText] string name)
        {
            var oldUsername = ctx.Client.CurrentUser.Username;
            if (string.IsNullOrWhiteSpace(name))
            {
                await ctx.Client.UpdateCurrentUserAsync(username: SharedData.Name);
                await BotServices.SendEmbedAsync(ctx, oldUsername + " username has been changed to " + SharedData.Name);
            }
            else
            {
                await ctx.Client.UpdateCurrentUserAsync(username: name);
                await BotServices.SendEmbedAsync(ctx, oldUsername + " username has been changed to " + ctx.Client.CurrentUser.Username, EmbedType.Good);
            }
        }

        #endregion COMMAND_USERNAME

        #endregion OWNERS-ONLY
    }
}