using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Modules.Bot;
using FlawBOT.Properties;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Discord
{
    [Group("server")]
    [Aliases("guild")]
    [Description("Commands for controlling server.")]
    [Cooldown(3, 5, CooldownBucketType.Guild)]
    public class ServerModule : BaseCommandModule
    {
        #region COMMAND_AVATAR

        [Command("avatar")]
        [Aliases("setavatar")]
        [Description("Change the server avatar.")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetServerAvatar(CommandContext ctx,
            [Description("URL image in JPG, PNG or IMG format.")]
            string query)
        {
            try
            {
                var stream = BotServices.CheckImageInput(ctx, query).Result;
                await ctx.Guild.ModifyAsync(chn => chn.Icon = stream).ConfigureAwait(false);
                await ctx.RespondAsync(ctx.Guild.Name + " server avatar has been updated!").ConfigureAwait(false);
            }
            catch
            {
                await BotServices
                    .SendResponseAsync(ctx, ctx.Guild.Name + " server avatar has not been updated!", ResponseType.Error)
                    .ConfigureAwait(false);
            }
        }

        #endregion COMMAND_AVATAR

        #region COMMAND_INFO

        [Command("info")]
        [Description("Retrieve server information.")]
        public async Task GetServer(CommandContext ctx)
        {
            var output = new DiscordEmbedBuilder()
                .WithAuthor($"Owner: {ctx.Guild.Owner.Username}#{ctx.Guild.Owner.Discriminator}",
                    iconUrl: ctx.Guild.Owner.AvatarUrl ?? string.Empty)
                .WithTitle(ctx.Guild.Name)
                .WithDescription("ID: " + ctx.Guild.Id)
                .AddField("Created on", ctx.Guild.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture),
                    true)
                .AddField("Member Count", ctx.Guild.MemberCount.ToString(), true)
                .AddField("Region", ctx.Guild.VoiceRegion.Name.ToUpperInvariant(), true)
                .AddField("Authentication", ctx.Guild.MfaLevel.ToString(), true)
                .AddField("Content Filter", ctx.Guild.ExplicitContentFilter.ToString(), true)
                .AddField("Verification", ctx.Guild.VerificationLevel.ToString(), true)
                .WithFooter(ctx.Guild.Name + " / #" + ctx.Channel.Name + " / " + DateTime.Now)
                .WithColor(DiscordColor.Rose);
            if (!string.IsNullOrEmpty(ctx.Guild.IconHash))
                output.WithThumbnail(ctx.Guild.IconUrl);

            var roles = new StringBuilder();
            foreach (var role in ctx.Guild.Roles)
                roles.Append($"[`{role.Value.Name}`]");
            if (roles.Length == 0) roles.Append("None");
            output.AddField("Roles", roles.ToString());

            var emojis = new StringBuilder();
            foreach (var emoji in ctx.Guild.Emojis)
                emojis.Append(emoji.Value.Name + (!emoji.Equals(ctx.Guild.Emojis.Last()) ? ", " : string.Empty));
            if (emojis.Length != 0) output.AddField("Emojis", emojis.ToString(), true);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_INFO

        #region COMMAND_INVITE

        [Command("invite")]
        [Description("Retrieve an instant invite link to the server.")]
        public async Task Invite(CommandContext ctx)
        {
            await ctx.RespondAsync("Instant Invite to " + Formatter.Bold(ctx.Guild.Name) + ":https://discord.gg/" +
                                   ctx.Channel.CreateInviteAsync().Result.Code).ConfigureAwait(false);
        }

        #endregion COMMAND_INVITE

        #region COMMAND_LEAVE

        [Command("leave")]
        [Description("Make FlawBOT leave the server.")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task LeaveServer(CommandContext ctx)
        {
            var settings = Program.Settings;
            await ctx.RespondAsync($"Are you sure you want {settings.Name} to leave the server?").ConfigureAwait(false);
            var message = await ctx.RespondAsync(Resources.INFO_RESPOND).ConfigureAwait(false);
            var interactivity = await BotServices.GetUserInteractivity(ctx, "yes", 10).ConfigureAwait(false);
            if (interactivity.Result is null)
            {
                await message.ModifyAsync($"~~{message.Content}~~ {Resources.INFO_REQ_TIMEOUT}").ConfigureAwait(false);
                return;
            }

            await BotServices.SendResponseAsync(ctx, $"Thank you for using {settings.Name}").ConfigureAwait(false);
            await ctx.Guild.LeaveAsync().ConfigureAwait(false);
        }

        #endregion COMMAND_LEAVE

        #region COMMAND_PRUNE

        [Command("prune")]
        [Description("Prune inactive server members.")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task Prune(CommandContext ctx,
            [Description("Number of days the user had to be inactive to get pruned.")]
            int days = 7)
        {
            if (days < 1 || days > 30)
                await BotServices
                    .SendResponseAsync(ctx, "Number of days must be between 1 and 30", ResponseType.Warning)
                    .ConfigureAwait(false);
            var count = await ctx.Guild.GetPruneCountAsync(days).ConfigureAwait(false);
            if (count == 0)
            {
                await ctx.RespondAsync("No inactive members found to prune").ConfigureAwait(false);
                return;
            }

            var prompt = await ctx
                .RespondAsync(
                    $"Pruning will remove {Formatter.Bold(count.ToString())} member(s).\nRespond with **yes** to continue.")
                .ConfigureAwait(false);
            var interactivity = await BotServices.GetUserInteractivity(ctx, "yes", 10).ConfigureAwait(false);
            if (interactivity.Result is null) return;
            await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            await BotServices.RemoveMessage(prompt).ConfigureAwait(false);
            await ctx.Guild.PruneAsync(days).ConfigureAwait(false);
        }

        #endregion COMMAND_PRUNE

        #region COMMAND_RENAME

        [Command("rename")]
        [Aliases("setname")]
        [Description("Change the server name.")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetServerName(CommandContext ctx,
            [Description("New server name.")] [RemainingText]
            string name = "")
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            {
                await BotServices
                    .SendResponseAsync(ctx, "Server name cannot be blank or over 100 characters!", ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            await ctx.Guild.ModifyAsync(srv => srv.Name = name).ConfigureAwait(false);
            await ctx.RespondAsync("Server name has been changed to " + Formatter.Bold(name)).ConfigureAwait(false);
        }

        #endregion COMMAND_RENAME

        #region COMMAND_REPORT

        [Hidden]
        [Command("report")]
        [Aliases("issue")]
        [Description("Report a problem with FlawBOT to the developer.")]
        public async Task ReportIssue(CommandContext ctx, [Description("Detailed description of the issue.")][RemainingText] string report)
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

        #endregion COMMAND_REPORT

        #region COMMAND_WARN

        [Command("warn")]
        [Aliases("scold")]
        [Description("Direct message user with a warning.")]
        public async Task Warn(CommandContext ctx,
            [Description("Server user to warn.")] DiscordMember member,
            [Description("Warning message.")] [RemainingText]
            string reason = null)
        {
            var output = new DiscordEmbedBuilder()
                .WithTitle("Warning received!")
                .WithDescription(Formatter.Bold(ctx.Guild.Name) + " has issued you a server warning!")
                .AddField("Sender:", ctx.Member.Username + "#" + ctx.Member.Discriminator, true)
                .AddField("Server Owner:", ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator, true)
                .WithThumbnail(ctx.Guild.IconUrl)
                .WithTimestamp(DateTime.Now)
                .WithColor(DiscordColor.Red);
            if (!string.IsNullOrWhiteSpace(reason)) output.AddField("Warning message:", reason);

            var dm = await member.CreateDmChannelAsync().ConfigureAwait(false);
            if (dm is null)
            {
                await BotServices.SendResponseAsync(ctx, "Unable to direct message this user", ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            await dm.SendMessageAsync(output.Build()).ConfigureAwait(false);
            await ctx.RespondAsync("Successfully sent a warning to " + Formatter.Bold(member.Username))
                .ConfigureAwait(false);
        }

        #endregion COMMAND_WARN
    }
}