using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Group("server")]
    [Aliases("guild")]
    [Description("Commands for controlling server")]
    [Cooldown(3, 5, CooldownBucketType.Guild)]
    public class ServerModule : BaseCommandModule
    {
        #region COMMAND_AVATAR

        [Command("avatar")]
        [Aliases("setavatar")]
        [Description("Set server avatar")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetServerAvatar(CommandContext ctx,
            [Description("Image URL. Must be in jpg, png or img format.")] string query)
        {
            try
            {
                var stream = BotServices.CheckImageInput(ctx, query).Result;
                await ctx.Guild.ModifyAsync(chn => chn.Icon = stream);
                await BotServices.SendEmbedAsync(ctx, ctx.Guild.Name + " server avatar has been updated!", EmbedType.Good);
            }
            catch
            {
                await BotServices.SendEmbedAsync(ctx, ctx.Guild.Name + " server avatar has not been updated!", EmbedType.Error);
            }
        }

        #endregion COMMAND_AVATAR

        #region COMMAND_INFO

        [Command("info")]
        [Aliases("i")]
        [Description("Retrieve server information")]
        public async Task GetServer(CommandContext ctx)
        {
            var output = new DiscordEmbedBuilder()
                .WithAuthor($"Owner: {ctx.Guild.Owner.Username}#{ctx.Guild.Owner.Discriminator}", iconUrl: string.IsNullOrEmpty(ctx.Guild.Owner.AvatarHash) ? null : ctx.Guild.Owner.AvatarUrl)
                .WithTitle(ctx.Guild.Name + $" (ID: {ctx.Guild.Id.ToString()})")
                .AddField("Created on", ctx.Guild.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Member Count", ctx.Guild.MemberCount.ToString(), true)
                .AddField("Region", ctx.Guild.VoiceRegion.Name.ToUpperInvariant(), true)
                .AddField("Authentication", ctx.Guild.MfaLevel.ToString(), true)
                .AddField("Content Filter", ctx.Guild.ExplicitContentFilter.ToString(), true)
                .AddField("Verification", ctx.Guild.VerificationLevel.ToString(), true)
                .WithFooter(ctx.Guild.Name + " / #" + ctx.Channel.Name + " / " + DateTime.Now)
                .WithColor(DiscordColor.Rose);
            if (!string.IsNullOrEmpty(ctx.Guild.IconHash))
                output.WithThumbnailUrl(ctx.Guild.IconUrl);

            var roles = new StringBuilder();
            foreach (var role in ctx.Guild.Roles)
                roles.Append($"[`{role.Value.Name}`]");
            if (roles.Length == 0) roles.Append("None");
            output.AddField("Roles", roles.ToString());

            var emojis = new StringBuilder();
            foreach (var emoji in ctx.Guild.Emojis)
                emojis.Append(emoji.Value.Name);
            if (emojis.Length != 0) output.AddField("Emojis", emojis.ToString(), true);
            await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_INFO

        #region COMMAND_INVITE

        [Command("invite")]
        [Description("Retrieve an instant invite link to the server")]
        public async Task InviteAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("Instant Invite to " + Formatter.Bold(ctx.Guild.Name) + ":https://discord.gg/" + ctx.Channel.CreateInviteAsync().Result.Code);
        }

        #endregion COMMAND_INVITE

        #region COMMAND_PRUNE

        [Command("prune")]
        [Description("Prune inactive server members")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task PruneUsers(CommandContext ctx,
            [Description("Number of days the user had to be inactive to get pruned")] [RemainingText] int days = 7)
        {
            if (days < 1 || days > 30)
                await BotServices.SendEmbedAsync(ctx, "Number of days must be between 1 and 30", EmbedType.Warning);
            int count = await ctx.Guild.GetPruneCountAsync(days);
            if (count == 0)
            {
                await BotServices.SendEmbedAsync(ctx, "No inactive members found to prune", EmbedType.Warning);
                return;
            }
            var prompt = await ctx.RespondAsync($"Pruning will remove {Formatter.Bold(count.ToString())} member(s).\nRespond with **yes** to continue.");
            var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "yes", TimeSpan.FromSeconds(10));
            if (interactivity.Result is null) return;
            await BotServices.RemoveMessage(interactivity.Result);
            await BotServices.RemoveMessage(prompt);
            await ctx.Guild.PruneAsync(days);
        }

        #endregion COMMAND_PRUNE

        #region COMMAND_RENAME

        [Command("rename")]
        [Aliases("setname")]
        [Description("Set server name")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetServerName(CommandContext ctx,
            [Description("New server name")] [RemainingText] string name = "")
        {
            if (string.IsNullOrWhiteSpace(name) || (name.Length > 100))
                await BotServices.SendEmbedAsync(ctx, "Server name cannot be blank or over 100 characters!", EmbedType.Warning);
            else
            {
                await ctx.Guild.ModifyAsync(srv => srv.Name = name);
                await BotServices.SendEmbedAsync(ctx, "Server name has been changed to " + Formatter.Bold(name), EmbedType.Good);
            }
        }

        #endregion COMMAND_RENAME

        #region COMMAND_WARN

        [Command("warn")]
        [Aliases("scold")]
        [Description("Direct message user with a warning")]
        public async Task Warn(CommandContext ctx,
            [Description("Server user to warn")] DiscordMember member,
            [Description("Warning message")] [RemainingText] string reason = null)
        {
            var output = new DiscordEmbedBuilder()
                .WithTitle("Warning received!")
                .WithDescription(Formatter.Bold(ctx.Guild.Name) + " has issued you a server warning!")
                .AddField("Sender:", ctx.Member.Username + "#" + ctx.Member.Discriminator, true)
                .AddField("Server Owner:", ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator, true)
                .WithThumbnailUrl(ctx.Guild.IconUrl)
                .WithTimestamp(DateTime.Now)
                .WithColor(DiscordColor.Red);
            if (!string.IsNullOrWhiteSpace(reason)) output.AddField("Warning message:", reason);
            var dm = await member.CreateDmChannelAsync().ConfigureAwait(false);
            if (dm is null)
                await BotServices.SendEmbedAsync(ctx, "Unable to direct message this user", EmbedType.Warning);
            else
            {
                await dm.SendMessageAsync(embed: output.Build()).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, "Successfully sent a warning to " + Formatter.Bold(member.Username), EmbedType.Good).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_WARN
    }
}