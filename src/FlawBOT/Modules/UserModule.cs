using DSharpPlus;
using DSharpPlus.CommandsNext;
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
    [SlashCommandGroup("user", "Slash command group for modal user commands.")]
    public class UserModule : ApplicationCommandModule
    {
        [SlashCommand("avatar", "Retrieve server user's profile picture.")]
        public async Task GetAvatar(InteractionContext ctx, [Option("member", "Server user whose profile picture to retrieve.")] DiscordMember member)
        {
            member ??= ctx.Member;
            var output = new DiscordEmbedBuilder()
                .WithImageUrl(member.AvatarUrl)
                .WithColor(DiscordColor.Lilac);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        [SlashCommand("ban", "Ban a server user.")]
        [SlashRequirePermissions(Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx, [Option("member", "Server user to ban.")] DiscordMember member, [Option("reason", "Reason for the ban.")] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
            {
                await ctx.RespondAsync("You cannot ban yourself.").ConfigureAwait(false);
                return;
            }

            await ctx.Guild.BanMemberAsync(member, 7, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Ban, member, reason ?? "No reason provided.").ConfigureAwait(false);
        }

        [SlashCommand("deafen", "Deafen a server user.")]
        [SlashRequirePermissions(Permissions.DeafenMembers)]
        public async Task DeafenUser(CommandContext ctx, [Option("member", "Server user to deafen.")] DiscordMember member, [Option("reason", "Reason for the deafen.")] string reason = null)
        {
            if (member.IsDeafened)
            {
                await ctx.RespondAsync($"{member.DisplayName}#{member.Discriminator} is already **deafened**.").ConfigureAwait(false);
                return;
            }

            await member.SetDeafAsync(true, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Deafen, member, reason ?? "No reason provided.").ConfigureAwait(false);
        }

        [SlashCommand("info", "Retrieve server user's information.")]
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
            if ((permsobj & Permissions.Administrator | permsobj & Permissions.AccessChannels) == 0)
                perms = $"**This user cannot see this channel!**\n{perms}";
            output.AddField("Permissions", perms ?? "*None*");
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        [SlashCommand("kick", "Kick a user from the server.")]
        [SlashRequirePermissions(Permissions.KickMembers)]
        public async Task Kick(CommandContext ctx, [Option("member", "Server user to kick.")] DiscordMember member, [Option("reason", "Reason for the kick.")] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
            {
                await ctx.RespondAsync("You cannot kick yourself.").ConfigureAwait(false);
                return;
            }

            await member.RemoveAsync(reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Kick, member, reason ?? "No reason provided.").ConfigureAwait(false);
        }

        [SlashCommand("mute", "Mute a server user.")]
        [SlashRequirePermissions(Permissions.MuteMembers)]
        public async Task Mute(CommandContext ctx, [Option("member", "Server user to mute.")] DiscordMember member, [Option("reason", "Reason for the mute.")] string reason = null)
        {
            if (member.IsMuted)
            {
                await ctx.RespondAsync($"{member.DisplayName}#{member.Discriminator} is already **muted**.").ConfigureAwait(false);
                return;
            }

            await member.SetMuteAsync(true, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Mute, member, reason ?? "No reason provided.").ConfigureAwait(false);
        }

        [SlashCommand("nickname", "Change server user's nickname.")]
        [RequireUserPermissions(Permissions.ChangeNickname)]
        public async Task SetUserName(InteractionContext ctx, [Option("member", "Server user name.")] DiscordMember member, [Option("name", "New nickname for the name.")] string name = null)
        {
            member ??= ctx.Member;
            var nickname = member.DisplayName;
            await member.ModifyAsync(usr => usr.Nickname = name).ConfigureAwait(false);
            var response = !string.IsNullOrWhiteSpace(name)
                ? $"{nickname}'s nickname has been changed to **{name}**"
                : $"{nickname}'s nickname has been reset.";
            await ctx.CreateResponseAsync(response).ConfigureAwait(false);
        }

        [SlashCommand("perms", "Retrieve server user's permissions.")]
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

        [SlashCommand("unban", "Unban a server user.")]
        [SlashRequirePermissions(Permissions.BanMembers)]
        public async Task Unban(CommandContext ctx, [Option("userId", "Discord user ID to unban from the server.")] ulong userId, [Option("reason", "Reason for the unban.")] string reason = null)
        {
            var member = await ctx.Client.GetUserAsync(userId).ConfigureAwait(false);
            await ctx.Guild.UnbanMemberAsync(member, reason ?? "No reason provided.").ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await ctx.RespondAsync($"Unbanned Discord User #{member} from the server.").ConfigureAwait(false);
        }

        [SlashCommand("undeafen", "Undeafen a server user.")]
        [SlashRequirePermissions(Permissions.DeafenMembers)]
        public async Task Undeafen(CommandContext ctx, [Option("member", "Server user to undeafen.")] DiscordMember member, [Option("reason", "Reason for the deafen.")] string reason = null)
        {
            await member.SetDeafAsync(false, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Undeafen, member, reason ?? "No reason provided").ConfigureAwait(false);
        }

        [SlashCommand("unmute", "Unmute a server user.")]
        [SlashRequirePermissions(Permissions.MuteMembers)]
        public async Task Unmute(CommandContext ctx, [Option("member", "Server user to unmute.")] DiscordMember member, [Option("reason", "Reason for the deafen.")] string reason = null)
        {
            await member.SetMuteAsync(false, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Unmute, member, reason ?? "No reason provided").ConfigureAwait(false);
        }
    }
}