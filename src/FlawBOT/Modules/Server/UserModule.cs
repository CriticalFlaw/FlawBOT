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
    [Description("Miscellaneous user control commands. Group call prints information about given user.")]
    [Aliases("users", "u", "usr")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class UserModule : BaseCommandModule
    {
        #region COMMAND_INFO

        [Command("info")]
        [Aliases("uid", "user")]
        [Description("Retrieve User Information")]
        public async Task GetUser(CommandContext ctx, [RemainingText] DiscordMember member)
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

        #region COMMAND_NICKNAME

        [Command("setnickname")]
        [Aliases("setnick")]
        [Description("Set server member's nickname")]
        [RequireUserPermissions(Permissions.ChangeNickname)]
        public async Task SetUserName(CommandContext ctx, string name, [RemainingText] DiscordMember member)
        {
            member = member ?? ctx.Member;
            var nickname = member.DisplayName;
            await member.ModifyAsync(usr => usr.Nickname = name);
            await BotServices.SendEmbedAsync(ctx, $"{nickname}'s nickname has been changed to **{name}**", EmbedType.Good);
        }

        #endregion COMMAND_NICKNAME

        #region COMMAND_BAN

        [Command("ban")]
        [Aliases("b")]
        [Description("Ban server user")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
                await BotServices.SendEmbedAsync(ctx, ":warning: You cannot ban yourself!", EmbedType.Warning);
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
        [Aliases("d")]
        [Description("Deafen server user")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task Deafen(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
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

        #region COMMAND_KICK

        [Command("kick")]
        [Aliases("k")]
        [Description("Kick server user")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task Kick(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
                await BotServices.SendEmbedAsync(ctx, ":warning: You cannot kick yourself!", EmbedType.Warning);
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
        [Aliases("m")]
        [Description("Mute server user")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Mute(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
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

        #region COMMAND_UNBAN

        [Command("unban")]
        [Aliases("unb")]
        [Description("Unban server user")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Remove(CommandContext ctx, ulong userID)
        {
            var member = await ctx.Client.GetUserAsync(userID).ConfigureAwait(false);
            await ctx.Guild.UnbanMemberAsync(member).ConfigureAwait(false);
            await BotServices.SendEmbedAsync(ctx, $"**Unbanned** user {member.Username}#{member.Discriminator} (ID:{member.Id})", EmbedType.Good);
        }

        #endregion COMMAND_UNBAN

        #region COMMAND_UNDEAFEN

        [Command("undeafen")]
        [Aliases("und")]
        [Description("Undeafen server user")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task Undeafen(CommandContext ctx, [RemainingText] DiscordMember member)
        {
            await member.SetDeafAsync(false);
            await BotServices.SendEmbedAsync(ctx, $"**Undeafened** user {member.DisplayName}#{member.Discriminator} (ID:{member.Id})", EmbedType.Good);
        }

        #endregion COMMAND_UNDEAFEN

        #region COMMAND_UNMUTE

        [Command("unmute")]
        [Aliases("unm")]
        [Description("Unmute server user")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Unmute(CommandContext ctx, [RemainingText] DiscordMember member)
        {
            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} (ID: {ctx.User.Id})";
            await member.SetMuteAsync(false, ustr);
            await BotServices.SendEmbedAsync(ctx, $"**Unmuted** user {member.Username}#{member.Discriminator} (ID:{member.Id})", EmbedType.Good);
        }

        #endregion COMMAND_UNMUTE

        #region COMMAND_AVATAR

        [Command("avatar")]
        [Aliases("av")]
        [Description("Get server user's avatar")]
        public async Task GetAvatar(CommandContext ctx, [RemainingText] DiscordMember member)
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
    }
}