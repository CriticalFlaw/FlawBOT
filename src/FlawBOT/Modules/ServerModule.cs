using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("server", "Slash command group for modal server commands.")]
    public class ServerModule : ApplicationCommandModule
    {
        [SlashCommand("avatar", "Change the server avatar.")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetServerAvatar(InteractionContext ctx, [Option("query", "URL image in JPG, PNG or IMG format.")] string query)
        {
            try
            {
                var stream = BotServices.CheckImageInput(ctx, query).Result;
                await ctx.Guild.ModifyAsync(chn => chn.Icon = stream).ConfigureAwait(false);
                await ctx.CreateResponseAsync(ctx.Guild.Name + " server avatar has been updated!").ConfigureAwait(false);
            }
            catch
            {
                await BotServices.SendResponseAsync(ctx, ctx.Guild.Name + " server avatar has not been updated!", ResponseType.Error).ConfigureAwait(false);
            }
        }

        [SlashCommand("info", "Retrieve server information.")]
        public async Task GetServer(InteractionContext ctx)
        {
            var output = new DiscordEmbedBuilder()
                .WithAuthor($"Owner: {ctx.Guild.Owner.Username}#{ctx.Guild.Owner.Discriminator}", iconUrl: ctx.Guild.Owner.AvatarUrl ?? string.Empty)
                .WithTitle(ctx.Guild.Name)
                .WithDescription("ID: " + ctx.Guild.Id)
                .AddField("Created on", ctx.Guild.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
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
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }
         
        [SlashCommand("invite", "Retrieve an instant invite link to the server.")]
        public async Task Invite(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("Instant Invite to " + Formatter.Bold(ctx.Guild.Name) + ":https://discord.gg/" + ctx.Channel.CreateInviteAsync().Result.Code).ConfigureAwait(false);
        }

        [SlashCommand("leave", "Make FlawBOT leave the server.")]
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

        [SlashCommand("prune", "Prune inactive server members.")]
        [SlashRequirePermissions(Permissions.DeafenMembers)]
        public async Task Prune(CommandContext ctx, [Option("days", "Number of days the user had to be inactive to get pruned.")] int days = 7)
        {
            if (days < 1 || days > 30)
                await BotServices.SendResponseAsync(ctx, "Number of days must be between 1 and 30", ResponseType.Warning).ConfigureAwait(false);

            var count = await ctx.Guild.GetPruneCountAsync(days).ConfigureAwait(false);
            if (count == 0)
            {
                await ctx.RespondAsync("No inactive members found to prune").ConfigureAwait(false);
                return;
            }

            var prompt = await ctx.RespondAsync($"Pruning will remove {Formatter.Bold(count.ToString())} member(s).\nRespond with **yes** to continue.").ConfigureAwait(false);
            var interactivity = await BotServices.GetUserInteractivity(ctx, "yes", 10).ConfigureAwait(false);
            if (interactivity.Result is null) return;
            await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            await BotServices.RemoveMessage(prompt).ConfigureAwait(false);
            await ctx.Guild.PruneAsync(days).ConfigureAwait(false);
        }

        [SlashCommand("rename", "Change the server name.")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetServerName(InteractionContext ctx, [Option("name", "New server name.")] string name = "")
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            {
                await BotServices.SendResponseAsync(ctx, "Server name cannot be blank or over 100 characters!", ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await ctx.Guild.ModifyAsync(srv => srv.Name = name).ConfigureAwait(false);
            await ctx.CreateResponseAsync("Server name has been changed to " + Formatter.Bold(name)).ConfigureAwait(false);
        }

        [Hidden]
        [SlashCommand("report", "Report a problem with FlawBOT to the developer.")]
        public async Task ReportIssue(CommandContext ctx, [Option("report", "Detailed description of the issue.")] string report)
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

        [SlashCommand("warn", "Direct message user with a warning.")]
        public async Task Warn(InteractionContext ctx, [Option("member", "Server user to warn.")] DiscordMember member, [Option("reason", "Warning message.")] string reason = null)
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
                await BotServices.SendResponseAsync(ctx, "Unable to direct message this user", ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await dm.SendMessageAsync(output.Build()).ConfigureAwait(false);
            await ctx.CreateResponseAsync("Successfully sent a warning to " + Formatter.Bold(member.Username)).ConfigureAwait(false);
        }
    }
}