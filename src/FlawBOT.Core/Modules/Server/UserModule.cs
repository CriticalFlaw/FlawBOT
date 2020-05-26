using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Group("user")]
    [Aliases("users", "u", "usr")]
    [Description("Commands for controlling server users")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class UserModule : BaseCommandModule
    {
        #region COMMAND_AVATAR

        [Command("avatar")]
        [Aliases("getavatar", "image", "pfp")]
        [Description("Retrieve server user's profile picture")]
        public async Task GetAvatar(CommandContext ctx,
            [Description("Server user whose profile picture to retrieve")] [RemainingText] DiscordMember member)
        {
            member = member ?? ctx.Member;
            var output = new DiscordEmbedBuilder()
                .WithImageUrl(member.AvatarUrl)
                .WithUrl("https://images.google.com/searchbyimage?image_url=" + member.AvatarUrl) // UNUSED
                .WithColor(DiscordColor.Lilac);
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
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
            {
                await ctx.RespondAsync("You cannot ban yourself.").ConfigureAwait(false);
            }
            else
            {
                await ctx.Guild.BanMemberAsync(member, 7, reason).ConfigureAwait(false);
                await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
                await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Ban, member, reason ?? "No reason provided.");
            }
        }

        #endregion COMMAND_BAN

        #region COMMAND_DEAFEN

        [Command("deafen")]
        [Aliases("deaf")]
        [Description("Deafen server user")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task Deafen(CommandContext ctx,
            [Description("Server user to deafen")] DiscordMember member,
            [Description("Reason for the deafen")] [RemainingText] string reason = null)
        {
            if (member.IsDeafened)
            {
                await ctx.RespondAsync($"{member.DisplayName}#{member.Discriminator} is already **deafened**.")
                    .ConfigureAwait(false);
            }
            else
            {
                await member.SetDeafAsync(true, reason).ConfigureAwait(false);
                await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
                await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Deafen, member, reason ?? "No reason provided.");
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
                .WithDescription("ID: " + member.Id)
                .AddField("Registered on", member.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Joined on", member.JoinedAt.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Nickname", member.Nickname ?? "None", true)
                .AddField("Muted?", member.IsMuted ? "Yes" : "No", true)
                .AddField("Deafened?", member.IsDeafened ? "Yes" : "No", true)
                .WithThumbnailUrl(member.AvatarUrl)
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
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_INFO

        #region COMMAND_KICK

        [Command("kick")]
        [Aliases("remove")]
        [Description("Kick server user")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task Kick(CommandContext ctx,
            [Description("Server user to kick")] DiscordMember member,
            [Description("Reason for the kick")] [RemainingText] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
            {
                await ctx.RespondAsync("You cannot kick yourself.").ConfigureAwait(false);
            }
            else
            {
                await member.RemoveAsync(reason).ConfigureAwait(false);
                await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
                await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Kick, member, reason ?? "No reason provided.");
            }
        }

        #endregion COMMAND_KICK

        #region COMMAND_MUTE

        [Command("mute")]
        [Aliases("silence")]
        [Description("Mute server user")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Mute(CommandContext ctx,
            [Description("Server user to mute")] DiscordMember member,
            [Description("Reason for the mute")] [RemainingText] string reason = null)
        {
            if (member.IsMuted)
            {
                await ctx.RespondAsync($"{member.DisplayName}#{member.Discriminator} is already **muted**.")
                    .ConfigureAwait(false);
            }
            else
            {
                await member.SetMuteAsync(true, reason).ConfigureAwait(false);
                await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
                await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Mute, member, reason ?? "No reason provided.");
            }
        }

        #endregion COMMAND_MUTE

        #region COMMAND_NICKNAME

        [Command("nickname")]
        [Aliases("setnick", "nick")]
        [Description("Set server user's nickname")]
        [RequireUserPermissions(Permissions.ChangeNickname)]
        public async Task SetUserName(CommandContext ctx,
            [Description("Server user to nickname")] DiscordMember member,
            [Description("The new nickname")] [RemainingText] string name = null)
        {
            member = member ?? ctx.Member;
            var nickname = member.DisplayName;
            await member.ModifyAsync(usr => usr.Nickname = name).ConfigureAwait(false);
            var response = !string.IsNullOrWhiteSpace(name)
                ? $"{nickname}'s nickname has been changed to **{name}**"
                : $"{nickname}'s nickname has been reset.";
            await BotServices.SendEmbedAsync(ctx, response, EmbedType.Good).ConfigureAwait(false);
        }

        #endregion COMMAND_NICKNAME

        #region COMMAND_PERMS

        [Command("perms")]
        [Aliases("prm")]
        [Description("Retrieve server user's permissions")]
        public async Task ListServerPermissions(CommandContext ctx,
            [Description("Server user whose permissions to retrieve")] DiscordMember member = null,
            [Description("Server channel")] DiscordChannel channel = null)
        {
            member = member ?? ctx.Member;
            channel = channel ?? ctx.Channel;
            var perms = Formatter.Bold(member.DisplayName) + " cannot access channel " + Formatter.Bold(channel.Name);
            if (member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels))
                perms = member.PermissionsIn(channel).ToPermissionString();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"Permissions for {member.Username} in #{channel.Name}:")
                .WithDescription(perms)
                .WithColor(DiscordColor.Turquoise);
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_PERMS

        #region COMMAND_UNBAN

        [Command("unban")]
        [Description("Unban server user")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Remove(CommandContext ctx,
            [Description("Discord user ID to unban from the server")] ulong userId,
            [Description("Reason for the deafen")] [RemainingText] string reason = null)
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
        [Description("Undeafen server user")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task Undeafen(CommandContext ctx,
            [Description("Server user to undeafen")] [RemainingText] DiscordMember member,
            [Description("Reason for the deafen")] [RemainingText] string reason = null)
        {
            await member.SetDeafAsync(false, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Undeafen, member, reason ?? "No reason provided");
        }

        #endregion COMMAND_UNDEAFEN

        #region COMMAND_UNMUTE

        [Command("unmute")]
        [Description("Unmute server user")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Unmute(CommandContext ctx,
            [Description("Server user to unmute")] [RemainingText] DiscordMember member,
            [Description("Reason for the deafen")] [RemainingText] string reason = null)
        {
            await member.SetMuteAsync(false, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Unmute, member, reason ?? "No reason provided");
        }

        #endregion COMMAND_UNMUTE
    }
}