using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class ModeratorModule : BaseCommandModule
    {
        [Command("ban")]
        [Aliases("b")]
        [Description("Ban server user")]
        [RequirePermissions(Permissions.BanMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Ban(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: You cannot ban yourself!");
            else
            {
                await ctx.TriggerTypingAsync();
                var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
                var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
                await ctx.Guild.BanMemberAsync(member, 7, $"{ustr}: {rstr}");
                await ctx.RespondAsync($"**Banned** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})\n**Reason:** {rstr}\n**Banned by: **{ctx.Member.DisplayName}");
            }
        }

        [Command("deafen")]
        [Aliases("d")]
        [Description("Deafen server user")]
        [RequirePermissions(Permissions.DeafenMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Deafen(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            if (member.IsDeafened)
                await BotServices.SendErrorEmbedAsync(ctx, $"{member.DisplayName}#{member.Discriminator} is already **deafened**!");
            else
            {
                await ctx.TriggerTypingAsync();
                var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
                await member.SetDeafAsync(true, rstr);
                await ctx.RespondAsync($"**Deafened** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})\n**Reason:** {rstr}\n**Deafened by: **{ctx.Member.DisplayName}");
            }
        }

        [Command("kick")]
        [Aliases("k")]
        [Description("Kick server user")]
        [RequirePermissions(Permissions.KickMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Kick(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: You cannot kick yourself!");
            else
            {
                await ctx.TriggerTypingAsync();
                var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
                var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
                await member.RemoveAsync($"{ustr}: {rstr}");
                await ctx.RespondAsync($"**Kicked** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})\n**Reason:** {rstr}");
            }
        }

        [Command("mute")]
        [Aliases("m")]
        [Description("Mute server user")]
        [RequirePermissions(Permissions.MuteMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Mute(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            if (member.IsMuted)
                await BotServices.SendErrorEmbedAsync(ctx, $"{member.DisplayName}#{member.Discriminator} is already **muted**!");
            else
            {
                await ctx.TriggerTypingAsync();
                var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
                var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
                await member.SetMuteAsync(true, $"{ustr}: {rstr}");
                await ctx.RespondAsync($"**Muted** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})\n**Reason:** {rstr}");
            }
        }

        [Command("prune")]
        [Description("Prune inactive server members")]
        [RequirePermissions(Permissions.DeafenMembers)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task PruneUsers(CommandContext ctx, string day)
        {
            if (int.TryParse(day, out var days))
            {
                await ctx.TriggerTypingAsync();
                await ctx.RespondAsync($"**{ctx.Guild.GetPruneCountAsync(days).Result}** server members have been pruned.");
                await ctx.Guild.PruneAsync(days);
            }
            else
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: Invalid number of days, try **.prune 30**");
        }

        [Command("purge")]
        [Aliases("p")]
        [Description("Purge server users' messages")]
        [RequirePermissions(Permissions.ManageMessages)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Purge(CommandContext ctx, DiscordMember member, int limit)
        {
            if (limit <= 0 || limit > 100)
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: Invalid number of messages to delete, must be in range of 1-100!");
            var messages = await ctx.Channel.GetMessagesAfterAsync(ctx.Message.Id, limit);
            var delete = messages.Where(message => !string.IsNullOrWhiteSpace(member.ToString()) && message.Author.Id == member.Id).ToList();
            await ctx.Channel.DeleteMessagesAsync(delete).ConfigureAwait(false);
            await ctx.RespondAsync($"Purged **{delete.Count}** messages by {member.Username}#{member.Discriminator} (ID:{member.Id})");
        }

        [Command("removerole")]
        [Aliases("rr")]
        [Description("Remove a role from mentioned user")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task RemoveUserRole(CommandContext ctx, DiscordMember member, [RemainingText] DiscordRole role)
        {
            if (member != null && role != null)
            {
                await ctx.TriggerTypingAsync();
                await member.RevokeRoleAsync(role);
                await ctx.RespondAsync($"{member.DisplayName} has been revoked the role **{role.Name}**");
            }
        }

        [Command("removeroles")]
        [Aliases("rrs")]
        [Description("Remove all roles from mentioned user")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task RemoveUserRoles(CommandContext ctx, DiscordMember member)
        {
            if (member.Roles.Max(r => r.Position) >= ctx.Member.Roles.Max(r => r.Position))
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: You are not authorised to remove roles from this user!");
            else
            {
                await ctx.TriggerTypingAsync();
                await member.ReplaceRolesAsync(Enumerable.Empty<DiscordRole>()).ConfigureAwait(false);
                await ctx.RespondAsync($"Removed all roles from {member.DisplayName}");
            }
        }

        [Command("unban")]
        [Aliases("unb")]
        [Description("Unban server user")]
        [RequirePermissions(Permissions.BanMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Remove(CommandContext ctx, ulong userID)
        {
            await ctx.TriggerTypingAsync();
            var member = await ctx.Client.GetUserAsync(userID).ConfigureAwait(false);
            await ctx.Guild.UnbanMemberAsync(member).ConfigureAwait(false);
            await ctx.RespondAsync($"**Unbanned** user {member.Username}#{member.Discriminator} (ID:{member.Id})");
        }

        [Command("undeafen")]
        [Aliases("und")]
        [Description("Undeafen server user")]
        [RequirePermissions(Permissions.DeafenMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Undeafen(CommandContext ctx, [RemainingText] DiscordMember member)
        {
            await ctx.TriggerTypingAsync();
            await member.SetDeafAsync(false);
            await ctx.RespondAsync($"**Undeafened** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})");
        }

        [Command("unmute")]
        [Aliases("unm")]
        [Description("Unmute server user")]
        [RequirePermissions(Permissions.MuteMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Unmute(CommandContext ctx, [RemainingText] DiscordMember member)
        {
            await ctx.TriggerTypingAsync();
            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} (ID: {ctx.User.Id})";
            await member.SetMuteAsync(false, ustr);
            await ctx.RespondAsync($"**Unmuted** user {member.Username}#{member.Discriminator} (ID:{member.Id})");
        }

        [Command("warn")]
        [Aliases("w")]
        [Description("Direct message user with a warning")]
        [Cooldown(1, 10, CooldownBucketType.User)]
        [Cooldown(3, 10, CooldownBucketType.Channel)]
        public async Task Warn(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            await ctx.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("Warning received!")
                .WithDescription($"Guild **{ctx.Guild.Name}** issued you a warning!")
                .WithTimestamp(DateTime.Now)
                .WithColor(DiscordColor.Red);
            if (!string.IsNullOrWhiteSpace(reason)) output.AddField("Warning message:", reason);
            output.AddField("Sender:", $"{ctx.Member.Username}#{ctx.Member.Discriminator}");
            var dm = await member.CreateDmChannelAsync().ConfigureAwait(false);
            if (dm == null)
                await ctx.RespondAsync("Unable to direct message this user");
            else
            {
                await dm.SendMessageAsync(embed: output.Build()).ConfigureAwait(false);
                await ctx.RespondAsync($"Successfully sent a warning to {Formatter.Bold(member.Username)}.").ConfigureAwait(false);
            }
        }
    }
}