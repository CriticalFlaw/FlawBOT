using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
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
        public async Task BanUser(CommandContext CTX, DiscordMember member, [RemainingText] string reason = null)
        {
            if (CTX.Member.Id == member.Id)
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} You cannot ban yourself...");
            else
            {
                await CTX.TriggerTypingAsync();
                var ustr = $"{CTX.User.Username}#{CTX.User.Discriminator} ({CTX.User.Id})";
                var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
                await CTX.Guild.BanMemberAsync(member, 7, $"{ustr}: {rstr}");
                await CTX.RespondAsync($"**Banned** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})\n**Reason:** {rstr}");
            }
        }

        [Command("deafen")]
        [Aliases("d")]
        [Description("Deafen server user")]
        [RequirePermissions(Permissions.DeafenMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task DeafenUser(CommandContext CTX, DiscordMember member, [RemainingText] string reason = null)
        {
            if (member.IsDeafened == true)
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} {member.DisplayName}#{member.Discriminator} is already **deafened**");
            else
            {
                await CTX.TriggerTypingAsync();
                var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
                await member.SetDeafAsync(true, rstr);
                await CTX.RespondAsync($"**Deafened** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})\n**Reason:** {rstr}");
            }
        }

        [Command("kick")]
        [Aliases("k")]
        [Description("Kick server user")]
        [RequirePermissions(Permissions.KickMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task KickUser(CommandContext CTX, DiscordMember member, [RemainingText] string reason = null)
        {
            if (CTX.Member.Id == member.Id)
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} You cannot kick yourself...");
            else
            {
                await CTX.TriggerTypingAsync();
                var ustr = $"{CTX.User.Username}#{CTX.User.Discriminator} ({CTX.User.Id})";
                var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
                await member.RemoveAsync($"{ustr}: {rstr}");
                await CTX.RespondAsync($"**Kicked** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})\n**Reason:** {rstr}");
            }
        }

        [Command("mute")]
        [Aliases("m")]
        [Description("Mute server user")]
        [RequirePermissions(Permissions.MuteMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task MuteUser(CommandContext CTX, DiscordMember member, [RemainingText] string reason = null)
        {
            if (member.IsMuted == true)
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} {member.DisplayName}#{member.Discriminator} is already **muted**");
            else
            {
                await CTX.TriggerTypingAsync();
                var ustr = $"{CTX.User.Username}#{CTX.User.Discriminator} ({CTX.User.Id})";
                var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
                await member.SetMuteAsync(true, $"{ustr}: {rstr}");
                await CTX.RespondAsync($"**Muted** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})\n**Reason:** {rstr}");
            }
        }

        [Command("purge")]
        [Aliases("p")]
        [Description("Purge server users' messages")]
        [RequirePermissions(Permissions.ManageMessages)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task PurgeUser(CommandContext CTX, DiscordMember member, int limit)
        {
            if (limit <= 0 || limit > 100)
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Invalid number of messages to delete (must be in range 1-100");
            var delete = new List<DiscordMessage>();
            var messages = await CTX.Channel.GetMessagesAsync(limit, CTX.Message.Id);
            foreach (var message in messages)
                if (!string.IsNullOrWhiteSpace(member.ToString()) && message.Author.Id == member.Id)
                    delete.Add(message);
            await CTX.Channel.DeleteMessagesAsync(delete).ConfigureAwait(false);
            await CTX.RespondAsync($"Purged **{delete.Count}** messages by {member.Username}#{member.Discriminator} (ID:{member.Id})");
        }

        [Command("unban")]
        [Aliases("unb")]
        [Description("Unban server user")]
        [RequirePermissions(Permissions.BanMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task RemoveBan(CommandContext CTX, [RemainingText] DiscordMember member)
        {
            await CTX.TriggerTypingAsync();
            await member.UnbanAsync(CTX.Guild);
            await CTX.RespondAsync($"**Unbanned** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})");
        }

        [Command("undeafen")]
        [Aliases("und")]
        [Description("Undeafen server user")]
        [RequirePermissions(Permissions.DeafenMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task UndeafenUser(CommandContext CTX, [RemainingText] DiscordMember member)
        {
            if (member.IsDeafened == false)
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} {member.Username}#{member.Discriminator} is already **undeafened**");
            else
            {
                await CTX.TriggerTypingAsync();
                await member.SetDeafAsync(false);
                await CTX.RespondAsync($"**Undeafened** user  {member.DisplayName}#{member.Discriminator} (ID:{member.Id})");
            }
        }

        [Command("unmute")]
        [Aliases("unm")]
        [Description("Unmute server user")]
        [RequirePermissions(Permissions.MuteMembers)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task UnmuteUser(CommandContext CTX, [RemainingText] DiscordMember member)
        {
            if (member.IsMuted == false)
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} {CTX.Member.Username}#{CTX.Member.Discriminator} is already **unmuted**");
            else
            {
                await CTX.TriggerTypingAsync();
                var ustr = $"{CTX.User.Username}#{CTX.User.Discriminator} ({CTX.User.Id})";
                await member.SetMuteAsync(false, $"{ustr}");
                await CTX.RespondAsync($"**Unmuted** user {member.Username}#{member.Discriminator} (ID:{member.Id})");
            }
        }

        [Command("warn")]
        [Aliases("w")]
        [Description("Direct message user with a warning")]
        [Cooldown(1, 60, CooldownBucketType.User)]
        public async Task WarnUser(CommandContext CTX, DiscordMember member, [RemainingText] string reason = null)
        {
            await CTX.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("Warning received!")
                .WithDescription($"Guild {Formatter.Bold(CTX.Guild.Name)} issued a warning to you through me.")
                .WithTimestamp(DateTime.Now)
                .WithColor(DiscordColor.Red);
            if (!string.IsNullOrWhiteSpace(reason))
                output.AddField("**Warning message:**", reason);
            output.AddField("**Sender:**", $"{CTX.Member.Username}#{CTX.Member.Discriminator}");
            var DM = await member.CreateDmChannelAsync().ConfigureAwait(false);
            if (DM == null)
                await CTX.RespondAsync("Unable to direct message this user");
            await DM.SendMessageAsync(embed: output.Build()).ConfigureAwait(false);
            await CTX.RespondAsync($"Successfully sent a warning to {Formatter.Bold(member.Username)}.").ConfigureAwait(false);
        }
    }
}