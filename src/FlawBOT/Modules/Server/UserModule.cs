using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Services;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Server
{
    [Group("user")]
    [Aliases("users", "usr")]
    [Description("Commands for controlling server users.")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class UserModule : BaseCommandModule
    {
        #region COMMAND_AVATAR

        [Command("avatar")]
        [Aliases("getavatar", "image", "pfp")]
        [Description("Retrieve server user's profile picture.")]
        public async Task GetAvatar(CommandContext ctx,
            [Description("Server user whose profile picture to retrieve.")] [RemainingText]
            DiscordMember member)
        {
            member ??= ctx.Member;
            var output = new DiscordEmbedBuilder()
                .WithImageUrl(member.AvatarUrl)
                .WithColor(DiscordColor.Lilac);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_AVATAR

        #region COMMAND_BAN

        [Command("ban")]
        [Description("Ban a server user.")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx,
            [Description("Server user to ban.")] DiscordMember member,
            [Description("Reason for the ban.")] [RemainingText]
            string reason = null)
        {
            if (ctx.Member.Id == member.Id)
            {
                await ctx.RespondAsync("You cannot ban yourself.").ConfigureAwait(false);
                return;
            }

            await ctx.Guild.BanMemberAsync(member, 7, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Ban, member,
                reason ?? "No reason provided.").ConfigureAwait(false);
        }

        #endregion COMMAND_BAN

        #region COMMAND_DEAFEN

        [Command("deafen")]
        [Aliases("deaf")]
        [Description("Deafen a server user.")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task DeafenUser(CommandContext ctx,
            [Description("Server user to deafen.")]
            DiscordMember member,
            [Description("Reason for the deafen.")] [RemainingText]
            string reason = null)
        {
            if (member.IsDeafened)
            {
                await ctx.RespondAsync($"{member.DisplayName}#{member.Discriminator} is already **deafened**.")
                    .ConfigureAwait(false);
                return;
            }

            await member.SetDeafAsync(true, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Deafen, member,
                reason ?? "No reason provided.").ConfigureAwait(false);
        }

        #endregion COMMAND_DEAFEN

        #region COMMAND_INFO

        [Command("info")]
        [Description("Retrieve server user's information.")]
        public async Task GetUser(CommandContext ctx,
            [Description("Server user whose information to retrieve.")] [RemainingText]
            DiscordMember member)
        {
            member ??= ctx.Member;
            var roles = new StringBuilder();
            var permsobj = member.PermissionsIn(ctx.Channel);
            var perms = permsobj.ToPermissionString();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"@{member.Username}#{member.Discriminator}")
                .WithDescription("ID: " + member.Id)
                .AddField("Registered on", member.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture),
                    true)
                .AddField("Joined on", member.JoinedAt.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Nickname", member.Nickname ?? "None", true)
                .AddField("Muted?", member.IsMuted ? "Yes" : "No", true)
                .AddField("Deafened?", member.IsDeafened ? "Yes" : "No", true)
                .WithThumbnail(member.AvatarUrl)
                .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                .WithColor(member.Color);
            if (member.IsBot)
                output.Title += " __[BOT]__ ";
            if (member.IsOwner)
                output.Title += " __[OWNER]__ ";
            output.AddField("Verified?", member.Verified == true ? "Yes" : "No", true);
            foreach (var role in member.Roles)
                roles.Append($"[`{role.Name}`] ");
            if (roles.Length > 0)
                output.AddField("Roles", roles.ToString(), true);
            if (((permsobj & Permissions.Administrator) | (permsobj & Permissions.AccessChannels)) == 0)
                perms = $"**This user cannot see this channel!**\n{perms}";
            output.AddField("Permissions", perms ?? "*None*");
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_INFO

        #region COMMAND_KICK

        [Command("kick")]
        [Aliases("remove")]
        [Description("Kick a user from the server.")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task Kick(CommandContext ctx,
            [Description("Server user to kick.")] DiscordMember member,
            [Description("Reason for the kick.")] [RemainingText]
            string reason = null)
        {
            if (ctx.Member.Id == member.Id)
            {
                await ctx.RespondAsync("You cannot kick yourself.").ConfigureAwait(false);
                return;
            }

            await member.RemoveAsync(reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Kick, member,
                reason ?? "No reason provided.").ConfigureAwait(false);
        }

        #endregion COMMAND_KICK

        #region COMMAND_MUTE

        [Command("mute")]
        [Aliases("silence")]
        [Description("Mute a server user.")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Mute(CommandContext ctx,
            [Description("Server user to mute.")] DiscordMember member,
            [Description("Reason for the mute.")] [RemainingText]
            string reason = null)
        {
            if (member.IsMuted)
            {
                await ctx.RespondAsync($"{member.DisplayName}#{member.Discriminator} is already **muted**.")
                    .ConfigureAwait(false);
                return;
            }

            await member.SetMuteAsync(true, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Mute, member,
                reason ?? "No reason provided.").ConfigureAwait(false);
        }

        #endregion COMMAND_MUTE

        #region COMMAND_NICKNAME

        [Command("nickname")]
        [Aliases("setnick", "nick")]
        [Description("Change server user's nickname.")]
        [RequireUserPermissions(Permissions.ChangeNickname)]
        public async Task SetUserName(CommandContext ctx,
            [Description("Server user name.")] DiscordMember member,
            [Description("New nickname for the name.")] [RemainingText]
            string name = null)
        {
            member ??= ctx.Member;
            var nickname = member.DisplayName;
            await member.ModifyAsync(usr => usr.Nickname = name).ConfigureAwait(false);
            var response = !string.IsNullOrWhiteSpace(name)
                ? $"{nickname}'s nickname has been changed to **{name}**"
                : $"{nickname}'s nickname has been reset.";
            await ctx.RespondAsync(response).ConfigureAwait(false);
        }

        #endregion COMMAND_NICKNAME

        #region COMMAND_PERMS

        [Command("perms")]
        [Description("Retrieve server user's permissions.")]
        public async Task GetPermissionsList(CommandContext ctx,
            [Description("Server user name.")] DiscordMember member = null,
            [Description("Server channel.")] DiscordChannel channel = null)
        {
            member ??= ctx.Member;
            channel ??= ctx.Channel;
            var perms = Formatter.Bold(member.DisplayName) + " cannot access channel " + Formatter.Bold(channel.Name);
            if (member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels))
                perms = member.PermissionsIn(channel).ToPermissionString();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"Permissions for {member.Username} in #{channel.Name}:")
                .WithDescription(perms)
                .WithColor(DiscordColor.Turquoise);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_PERMS

        #region COMMAND_UNBAN

        [Command("unban")]
        [Description("Unban a server user.")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Unban(CommandContext ctx,
            [Description("Discord user ID to unban from the server.")]
            ulong userId,
            [Description("Reason for the unban.")] [RemainingText]
            string reason = null)
        {
            var member = await ctx.Client.GetUserAsync(userId).ConfigureAwait(false);
            await ctx.Guild.UnbanMemberAsync(member, reason ?? "No reason provided.").ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await ctx.RespondAsync($"Unbanned Discord User #{member} from the server.").ConfigureAwait(false);
        }

        #endregion COMMAND_UNBAN

        #region COMMAND_UNDEAFEN

        [Command("undeafen")]
        [Aliases("undeaf")]
        [Description("Undeafen a server user.")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task Undeafen(CommandContext ctx,
            [Description("Server user to undeafen.")]
            DiscordMember member,
            [Description("Reason for the deafen.")] [RemainingText]
            string reason = null)
        {
            await member.SetDeafAsync(false, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Undeafen, member,
                reason ?? "No reason provided").ConfigureAwait(false);
        }

        #endregion COMMAND_UNDEAFEN

        #region COMMAND_UNMUTE

        [Command("unmute")]
        [Description("Unmute a server user.")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Unmute(CommandContext ctx,
            [Description("Server user to unmute.")]
            DiscordMember member,
            [Description("Reason for the deafen.")] [RemainingText]
            string reason = null)
        {
            await member.SetMuteAsync(false, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Unmute, member,
                reason ?? "No reason provided").ConfigureAwait(false);
        }

        #endregion COMMAND_UNMUTE
    }
}