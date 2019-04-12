using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Services;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Server
{
    [Group("user")]
    [Aliases("users", "u", "usr")]
    [Description("Commands for controlling server users")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class UserModule : BaseCommandModule
    {
        #region COMMAND_AVATAR

        [Command("avatar")]
        [Aliases("getavatar")]
        [Description("Retrieve server user's avatar")]
        public async Task GetAvatar(CommandContext ctx,
            [Description("Server user whose avatar to retrieve")] [RemainingText] DiscordMember member)
        {
            member = member ?? ctx.Member;
            var output = new DiscordEmbedBuilder()
                .WithTitle($"@{member.DisplayName}'s avatar...")
                .WithImageUrl(member.AvatarUrl)
                .WithUrl($"https://images.google.com/searchbyimage?image_url={member.AvatarUrl}")
                .WithColor(DiscordColor.Lilac);
            await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_AVATAR

        #region COMMAND_BAN

        [Command("ban")]
        [Description("Ban server user")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx,
            [Description("Server user to ban")] DiscordMember member,
            [Description("Reason for the ban")] [RemainingText] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
                await BotServices.SendEmbedAsync(ctx, "You cannot ban yourself!", EmbedType.Warning);
            else
            {
                var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
                var rstr = string.IsNullOrWhiteSpace(reason) ? "No reason provided" : $": {reason}";
                await ctx.Guild.BanMemberAsync(member, 7, $"{ustr}: {rstr}");
                await BotServices.SendEmbedAsync(ctx, $"**Banned** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})\n**Reason:** {rstr}\n**Banned by: **{ctx.Member.DisplayName}", EmbedType.Good);
            }
        }

        #endregion COMMAND_BAN

        #region COMMAND_DEAFEN

        [Command("deafen")]
        [Description("Deafen server user")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task Deafen(CommandContext ctx,
            [Description("Server user to deafen")] DiscordMember member,
            [Description("Reason for the deafen")] [RemainingText] string reason = null)
        {
            if (member.IsDeafened)
                await BotServices.SendEmbedAsync(ctx, $"{member.DisplayName}#{member.Discriminator} is already **deafened**!", EmbedType.Warning);
            else
            {
                var rstr = string.IsNullOrWhiteSpace(reason) ? "No reason provided" : $": {reason}";
                await member.SetDeafAsync(true, rstr);
                await BotServices.SendEmbedAsync(ctx, $"**Deafened** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})\n**Reason:** {rstr}\n**Deafened by: **{ctx.Member.DisplayName}", EmbedType.Good);
            }
        }

        #endregion COMMAND_DEAFEN

        #region COMMAND_INFO

        [Command("info")]
        [Aliases("i")]
        [Description("Retrieve user information")]
        public async Task GetUser(CommandContext ctx,
            [Description("Server user whose information to retrieve")] [RemainingText] DiscordMember member)
        {
            member = member ?? ctx.Member;
            var roles = new StringBuilder();
            var permsobj = member.PermissionsIn(ctx.Channel);
            var perms = permsobj.ToPermissionString();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"@{member.Username}#{member.Discriminator}")
                .WithDescription("Nickname: ")
                .AddField("ID", member.Id.ToString(), true)
                .AddField("Registered on", member.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Joined on", member.JoinedAt.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Muted?", member.IsMuted ? "YES" : "NO", true)
                .AddField("Deafened?", member.IsDeafened ? "YES" : "NO", true)
                .WithThumbnailUrl(member.AvatarUrl)
                .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                .WithColor(member.Color);
            if (member.IsBot)
                output.Title += " __[BOT]__ ";
            if (member.IsOwner)
                output.Title += " __[OWNER]__ ";
            output.AddField("Verified?", member.Verified == true ? "YES" : "NO", true);
            output.AddField("Secured?", member.MfaEnabled == true ? "YES" : "NO", true);
            if (!string.IsNullOrWhiteSpace(member.Nickname))
                output.Description += member.Nickname;
            foreach (var role in member.Roles)
                roles.Append($"[`{role.Name}`] ");
            if (roles.Length == 0)
                roles.Append("*None*");
            output.AddField("Roles", roles.ToString(), true);
            if (((permsobj & Permissions.Administrator) | (permsobj & Permissions.AccessChannels)) == 0)
                perms = $"**This user cannot see this channel!**\n{perms}";
            if (string.IsNullOrWhiteSpace(perms))
                perms = "*None*";
            output.AddField("Permissions", perms);
            await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_INFO

        #region COMMAND_KICK

        [Command("kick")]
        [Description("Kick server user")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task Kick(CommandContext ctx,
            [Description("Server user to kick")] DiscordMember member,
            [Description("Reason for the kick")] [RemainingText] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
                await BotServices.SendEmbedAsync(ctx, "You cannot kick yourself!", EmbedType.Warning);
            else
            {
                var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
                var rstr = string.IsNullOrWhiteSpace(reason) ? "No reason provided" : $": {reason}";
                await member.RemoveAsync($"{ustr}: {rstr}");
                await BotServices.SendEmbedAsync(ctx, $"**Kicked** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})\n**Reason:** {rstr}", EmbedType.Good);
            }
        }

        #endregion COMMAND_KICK

        #region COMMAND_MUTE

        [Command("mute")]
        [Description("Mute server user")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Mute(CommandContext ctx,
            [Description("Server user to mute")] DiscordMember member,
            [Description("Reason for the mute")] [RemainingText] string reason = null)
        {
            if (member.IsMuted)
                await BotServices.SendEmbedAsync(ctx, $"{member.DisplayName}#{member.Discriminator} is already **muted**!", EmbedType.Warning);
            else
            {
                var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
                var rstr = string.IsNullOrWhiteSpace(reason) ? "No reason provided" : $": {reason}";
                await member.SetMuteAsync(true, $"{ustr}: {rstr}");
                await BotServices.SendEmbedAsync(ctx, $"**Muted** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})\n**Reason:** {rstr}", EmbedType.Good);
            }
        }

        #endregion COMMAND_MUTE

        #region COMMAND_NICKNAME

        [Command("nickname")]
        [Aliases("setnick")]
        [Description("Set server user's nickname")]
        [RequireUserPermissions(Permissions.ChangeNickname)]
        public async Task SetUserName(CommandContext ctx,
            [Description("Server user to nickname")] DiscordMember member,
            [Description("The new nickname")] [RemainingText] string name)
        {
            member = member ?? ctx.Member;
            var nickname = member.DisplayName;
            await member.ModifyAsync(usr => usr.Nickname = name);
            await BotServices.SendEmbedAsync(ctx, $"{nickname}'s nickname has been changed to **{name}**", EmbedType.Good);
        }

        #endregion COMMAND_NICKNAME

        #region COMMAND_PERMS

        [Command("perms")]
        [Aliases("prm")]
        [Description("Retrieve server user's permissions")]
        public async Task ListServerPermissions(CommandContext ctx,
            [Description("Server user whose permissions to retrieve")] DiscordMember member,
            [Description("Server channel")] DiscordChannel channel = null)
        {
            member = member ?? ctx.Member;
            channel = channel ?? ctx.Channel;
            var perms = $"{Formatter.Bold(member.DisplayName)} cannot access channel {Formatter.Bold(channel.Name)}.";
            if (member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels))
                perms = member.PermissionsIn(channel).ToPermissionString();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"Permissions for {member.Username} in #{channel.Name}:")
                .WithDescription(perms)
                .WithColor(DiscordColor.Turquoise);
            await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_PERMS

        #region COMMAND_UNBAN

        [Command("unban")]
        [Description("Unban server user")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Remove(CommandContext ctx,
            [Description("Discord user ID to unban from the server")] ulong userID)
        {
            var member = await ctx.Client.GetUserAsync(userID).ConfigureAwait(false);
            await ctx.Guild.UnbanMemberAsync(member).ConfigureAwait(false);
            await BotServices.SendEmbedAsync(ctx, $"**Unbanned** user {member.Username}#{member.Discriminator} (ID:{member.Id})", EmbedType.Good);
        }

        #endregion COMMAND_UNBAN

        #region COMMAND_UNDEAFEN

        [Command("undeafen")]
        [Description("Undeafen server user")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task Undeafen(CommandContext ctx,
            [Description("Server user to undeafen")] [RemainingText] DiscordMember member)
        {
            await member.SetDeafAsync(false);
            await BotServices.SendEmbedAsync(ctx, $"**Undeafened** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})", EmbedType.Good);
        }

        #endregion COMMAND_UNDEAFEN

        #region COMMAND_UNMUTE

        [Command("unmute")]
        [Description("Unmute server user")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Unmute(CommandContext ctx,
            [Description("Server user to unmute")] [RemainingText] DiscordMember member)
        {
            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} (ID: {ctx.User.Id})";
            await member.SetMuteAsync(false, ustr);
            await BotServices.SendEmbedAsync(ctx, $"**Unmuted** user {member.Username}#{member.Discriminator} (ID:{member.Id})", EmbedType.Good);
        }

        #endregion COMMAND_UNMUTE
    }
}