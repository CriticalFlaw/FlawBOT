using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class ModeratorModule
    {
        [Command("ban")]
        [Aliases("b")]
        [Description("Ban server user")]
        [RequirePermissions(Permissions.BanMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task BanUser(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
                await ctx.RespondAsync(":warning: You cannot ban yourself! :warning:");
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
        public async Task DeafenUser(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            if (member.IsDeafened)
                await ctx.RespondAsync($"{member.DisplayName}#{member.Discriminator} is already **deafened**!");
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
        public async Task KickUser(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
                await ctx.RespondAsync(":warning: You cannot kick yourself! :warning:");
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
        public async Task MuteUser(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            if (member.IsMuted)
                await ctx.RespondAsync($"{member.DisplayName}#{member.Discriminator} is already **muted**!");
            else
            {
                await ctx.TriggerTypingAsync();
                var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
                var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
                await member.SetMuteAsync(true, $"{ustr}: {rstr}");
                await ctx.RespondAsync($"**Muted** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})\n**Reason:** {rstr}");
            }
        }

        [Command("purge")]
        [Aliases("p")]
        [Description("Purge server users' messages")]
        [RequirePermissions(Permissions.ManageMessages)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task PurgeUser(CommandContext ctx, DiscordMember member, int limit)
        {
            if (limit <= 0 || limit > 100)
                await ctx.RespondAsync("Invalid number of messages to delete (must be in range 1-100)!");
            var messages = await ctx.Channel.GetMessagesAsync(limit, ctx.Message.Id);
            var delete = messages.Where(message => !string.IsNullOrWhiteSpace(member.ToString()) && message.Author.Id == member.Id).ToList();
            await ctx.Channel.DeleteMessagesAsync(delete).ConfigureAwait(false);
            await ctx.RespondAsync($"Purged **{delete.Count}** messages by {member.Username}#{member.Discriminator} (ID:{member.Id})");
        }

        [Command("unban")]
        [Aliases("unb")]
        [Description("Unban server user")]
        [RequirePermissions(Permissions.BanMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task RemoveBan(CommandContext ctx, ulong userID)
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
        public async Task UndeafenUser(CommandContext ctx, [RemainingText] DiscordMember member)
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
        public async Task UnmuteUser(CommandContext ctx, [RemainingText] DiscordMember member)
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
        public async Task WarnUser(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            await ctx.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("Warning received!")
                .WithDescription($"Guild **{ctx.Guild.Name}** issued you a warning!")
                .WithTimestamp(DateTime.Now)
                .WithColor(DiscordColor.Red);
            if (!string.IsNullOrWhiteSpace(reason))
                output.AddField("Warning message:", reason);
            output.AddField("Sender:", $"{ctx.Member.Username}#{ctx.Member.Discriminator}");
            var dm = await member.CreateDmChannelAsync().ConfigureAwait(false);
            if (dm == null)
                await ctx.RespondAsync("Unable to direct message this user");
            if (dm != null)
            {
                await dm.SendMessageAsync(embed: output.Build()).ConfigureAwait(false);
                await ctx.RespondAsync($"Successfully sent a warning to {Formatter.Bold(member.Username)}.").ConfigureAwait(false);
            }
        }
    }
}