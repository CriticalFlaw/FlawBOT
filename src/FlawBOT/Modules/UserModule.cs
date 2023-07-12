using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using FlawBOT.Common;
using FlawBOT.Services;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("user", "Slash command group for user commands.")]
    public class UserModule : ApplicationCommandModule
    {
        [SlashCommand("profile-pic", "Returns server user's profile picture.")]
        public async Task GetAvatar(InteractionContext ctx, [Option("member", "Server user whose profile picture to retrieve.")] DiscordUser user)
        {
            user ??= ctx.User;
            var output = new DiscordEmbedBuilder()
                .WithImageUrl(user.AvatarUrl)
                .WithColor(DiscordColor.Lilac);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        [SlashCommand("ban", "Bans a server user.")]
        [SlashRequirePermissions(Permissions.BanMembers)]
        public async Task BanUser(InteractionContext ctx,
            [Option("user", "Server user to ban.")] DiscordUser user,
            [Choice("None", 0)][Choice("1 Day", 1)][Choice("1 Week", 7)] [Option("deletedays", "Number of days of message history to delete")] long deleteDays = 0,
            [Option("reason", "Reason for the ban.")] string reason = null)
        {
            if (ctx.Member.Id == user.Id)
            {
                await ctx.CreateResponseAsync("You cannot ban yourself.").ConfigureAwait(false);
                return;
            }

            await ctx.Guild.BanMemberAsync(user.Id, (int)deleteDays, reason).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{user.Username} has been **banned** from the server.").ConfigureAwait(false);
        }

        [SlashCommand("deafen", "Deafens a server user.")]
        [SlashRequirePermissions(Permissions.DeafenMembers)]
        public async Task DeafenUser(InteractionContext ctx, [Option("member", "Server user to deafen.")] DiscordMember member, [Option("reason", "Reason for the deafen.")] string reason = null)
        {
            if (member.IsDeafened)
            {
                await ctx.CreateResponseAsync($"{member.DisplayName}#{member.Discriminator} is already **deafened**.").ConfigureAwait(false);
                return;
            }

            await member.SetDeafAsync(true, reason).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{member} has been **deafened**.").ConfigureAwait(false);
        }

        [SlashCommand("info", "Returns server user's information.")]
        public async Task GetUser(InteractionContext ctx, [Option("member", "Server user whose information to retrieve.")] DiscordMember member)
        {
            member ??= ctx.Member;
            var roles = new StringBuilder();
            var permsobj = member.PermissionsIn(ctx.Channel);
            var perms = permsobj.ToPermissionString();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"@{member.Username}#{member.Discriminator}")
                .WithDescription("ID: " + member.Id)
                .AddField("Registered on", member.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Joined on", member.JoinedAt.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Nickname", member.Nickname ?? "None", true)
                .AddField("Muted?", member.IsMuted ? ":heavy_check_mark:" : ":x:", true)
                .AddField("Deafened?", member.IsDeafened ? ":heavy_check_mark:" : ":x:", true)
                .WithThumbnail(member.AvatarUrl)
                .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                .WithColor(member.Color);
            if (member.IsBot)
                output.Title += " __[BOT]__ ";
            if (member.IsOwner)
                output.Title += " __[OWNER]__ ";
            output.AddField("Verified?", member.Verified == true ? ":heavy_check_mark:" : ":x:", true);
            foreach (var role in member.Roles)
                roles.Append($"[`{role.Name}`] ");
            if (roles.Length > 0)
                output.AddField("Roles", roles.ToString(), true);
            if ((permsobj & Permissions.Administrator | permsobj & Permissions.AccessChannels) == 0)
                perms = $"**This user cannot see this channel!**\n{perms}";
            output.AddField("Permissions", perms ?? "*None*");
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        [SlashCommand("kick", "Kicks a user from the server.")]
        [SlashRequirePermissions(Permissions.KickMembers)]
        public async Task Kick(InteractionContext ctx, [Option("member", "Server user to kick.")] DiscordMember member, [Option("reason", "Reason for the kick.")] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
            {
                await ctx.CreateResponseAsync("You cannot kick yourself.").ConfigureAwait(false);
                return;
            }

            await member.RemoveAsync(reason).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{member} has been **kicked** from the server.").ConfigureAwait(false);
        }

        [SlashCommand("mute", "Mutes a server user.")]
        [SlashRequirePermissions(Permissions.MuteMembers)]
        public async Task Mute(InteractionContext ctx, [Option("member", "Server user to mute.")] DiscordMember member, [Option("reason", "Reason for the mute.")] string reason = null)
        {
            if (member.IsMuted)
            {
                await ctx.CreateResponseAsync($"{member.DisplayName}#{member.Discriminator} is already **muted**.").ConfigureAwait(false);
                return;
            }

            await member.SetMuteAsync(true, reason).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{member} has been **muted**.").ConfigureAwait(false);
        }

        [SlashCommand("nickname", "Changes server user's nickname.")]
        [RequireUserPermissions(Permissions.ChangeNickname)]
        public async Task SetUserName(InteractionContext ctx, [Option("member", "Server user name.")] DiscordMember member, [Option("name", "New nickname for the name.")] string name = null)
        {
            member ??= ctx.Member;
            var nickname = member.DisplayName;
            await member.ModifyAsync(usr => usr.Nickname = name).ConfigureAwait(false);
            var response = !string.IsNullOrWhiteSpace(name)
                ? $"{nickname}'s nickname has been changed to {Formatter.Bold(name)}"
                : $"{nickname}'s nickname has been reset.";
            await ctx.CreateResponseAsync(response).ConfigureAwait(false);
        }

        [SlashCommand("perms", "Returns server user's permissions.")]
        public async Task GetPermissionsList(InteractionContext ctx, [Option("member", "Server user name.")] DiscordMember member = null, [Option("channel", "Server channel.")] DiscordChannel channel = null)
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
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        [SlashCommand("unban", "Unbans a user from the server.")]
        [SlashRequirePermissions(Permissions.BanMembers)]
        public async Task Unban(InteractionContext ctx, [Option("userId", "Discord user ID to unban from the server.")] ulong userId, [Option("reason", "Reason for the unban.")] string reason = null)
        {
            var member = await ctx.Client.GetUserAsync(userId).ConfigureAwait(false);
            await ctx.Guild.UnbanMemberAsync(member, reason ?? "No reason provided.").ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{member} has been **unbanned** from the server.").ConfigureAwait(false);
        }

        [SlashCommand("undeafen", "Undeafen a server user.")]
        [SlashRequirePermissions(Permissions.DeafenMembers)]
        public async Task Undeafen(InteractionContext ctx, [Option("member", "Server user to undeafen.")] DiscordMember member, [Option("reason", "Reason for the deafen.")] string reason = null)
        {
            await member.SetDeafAsync(false, reason).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{member} has been **undeafened**.").ConfigureAwait(false);
        }

        [SlashCommand("unmute", "Unmutes a server user.")]
        [SlashRequirePermissions(Permissions.MuteMembers)]
        public async Task Unmute(InteractionContext ctx, [Option("member", "Server user to unmute.")] DiscordMember member, [Option("reason", "Reason for the deafen.")] string reason = null)
        {
            await member.SetMuteAsync(false, reason).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{member} has been **unmuted**.").ConfigureAwait(false);
        }

        [SlashCommand("warn", "Direct messages user with a warning.")]
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