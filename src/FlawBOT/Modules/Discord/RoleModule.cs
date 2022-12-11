using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Group("role")]
    [Aliases("roles")]
    [Description("Commands for controlling server roles.")]
    [Cooldown(3, 5, CooldownBucketType.Guild)]
    public class RoleModule : BaseCommandModule
    {
        #region COMMAND_COLOR

        [Command("color")]
        [Aliases("setcolor", "clr")]
        [Description("Change the server role color.")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task ColorRole(CommandContext ctx,
            [Description("Server role's new HEX color code.")]
            DiscordColor color,
            [Description("Server role to recolor.")] [RemainingText]
            DiscordRole role)
        {
            var regex = new Regex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", RegexOptions.Compiled).Match(color.ToString());
            if (!regex.Success)
            {
                await BotServices.SendResponseAsync(ctx,
                    "Invalid color code. Please enter a HEX color code like #E7B53B",
                    ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await role.ModifyAsync(color: color).ConfigureAwait(false);
            var output = new DiscordEmbedBuilder()
                .WithTitle("Successfully set the color for the role " + Formatter.Bold(role.Name) + " to " +
                           Formatter.InlineCode(role.Color.ToString()))
                .WithColor(color);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_COLOR

        #region COMMAND_CREATE

        [Command("create")]
        [Aliases("new", "add")]
        [Description("Create a new server role.")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task CreateRole(CommandContext ctx,
            [Description("New role name.")] [RemainingText]
            string role = "")
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_NAME, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            await ctx.Guild.CreateRoleAsync(role).ConfigureAwait(false);
            await ctx.RespondAsync("Successfully created the server role " + Formatter.Bold(role))
                .ConfigureAwait(false);
        }

        #endregion COMMAND_CREATE

        #region COMMAND_DELETE

        [Command("delete")]
        [Aliases("remove")]
        [Description("Delete a server role.")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task DeleteRole(CommandContext ctx,
            [Description("Server role to remove.")] [RemainingText]
            DiscordRole role = null)
        {
            if (role is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_EXISTING, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            await role.DeleteAsync().ConfigureAwait(false);
            await ctx.RespondAsync("Successfully removed the server role " + Formatter.Bold(role.Name))
                .ConfigureAwait(false);
        }

        #endregion COMMAND_DELETE

        #region COMMAND_INFO

        [Command("info")]
        [Description("Retrieve server role information.")]
        public async Task GetRole(CommandContext ctx,
            [Description("Server role name.")] [RemainingText]
            DiscordRole role = null)
        {
            if (role is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_EXISTING, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            var output = new DiscordEmbedBuilder()
                .WithTitle(role.Name)
                .WithDescription("ID: " + role.Id)
                .AddField("Creation Date", role.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture),
                    true)
                .AddField("Hoisted", role.IsHoisted ? "Yes" : "No", true)
                .AddField("Mentionable", role.IsMentionable ? "Yes" : "No", true)
                .AddField("Permissions", role.Permissions.ToPermissionString())
                .WithThumbnail(ctx.Guild.IconUrl)
                .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                .WithColor(role.Color);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_INFO

        #region COMMAND_INROLE

        [Command("inrole")]
        [Description("Retrieve a list of users with a given role.")]
        public async Task UsersInRole(CommandContext ctx,
            [Description("Server role name.")] [RemainingText]
            DiscordRole role = null)
        {
            if (role is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_EXISTING, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            var userCount = 0;
            var usersList = new StringBuilder();
            var users = (await ctx.Guild.GetAllMembersAsync().ConfigureAwait(false)).ToArray();
            foreach (var user in users)
                if (user.Roles.Contains(role))
                {
                    userCount++;
                    if (user.Equals(users.Last()))
                        usersList.Append(user.DisplayName);
                    else
                        usersList.Append(user.DisplayName).Append(", ");
                }

            if (usersList.Length == 0)
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " has no members")
                    .ConfigureAwait(false);
            else
                await BotServices
                    .SendResponseAsync(ctx, Formatter.Bold(role.Name) + $" has **{userCount}** member(s): {usersList}")
                    .ConfigureAwait(false);
        }

        #endregion COMMAND_INROLE

        #region COMMAND_MENTION

        [Command("mention")]
        [Description("Toggle whether this role can be mentioned by others.")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task MentionRole(CommandContext ctx,
            [Description("Server role name.")] [RemainingText]
            DiscordRole role)
        {
            if (role is null) return;
            if (role.IsMentionable)
            {
                await role.ModifyAsync(mentionable: false).ConfigureAwait(false);
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " is now **not-mentionable**")
                    .ConfigureAwait(false);
            }
            else
            {
                await role.ModifyAsync(mentionable: true).ConfigureAwait(false);
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " is now **mentionable**")
                    .ConfigureAwait(false);
            }
        }

        #endregion COMMAND_MENTION

        #region COMMAND_REVOKE

        [Command("revoke")]
        [Description("Remove a role from server user.")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task RevokeRole(CommandContext ctx,
            [Description("Server user to get revoked.")]
            DiscordMember member,
            [Description("Server role name.")] [RemainingText]
            DiscordRole role)
        {
            if (role != null)
            {
                member ??= ctx.Member;
                await member.RevokeRoleAsync(role).ConfigureAwait(false);
                await ctx.RespondAsync(Formatter.Bold(member.DisplayName) + " has been removed from the role " +
                                       Formatter.Bold(role.Name)).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_REVOKE

        #region COMMAND_REVOKE_ALL

        [Command("revokeall")]
        [Description("Remove all role from a server user.")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task RevokeAllRoles(CommandContext ctx,
            [Description("Server user name.")] DiscordMember member)
        {
            if (!member.Roles.Any())
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_NONE, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            if (member.Roles.Max(r => r.Position) >= ctx.Member.Roles.Max(r => r.Position))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_NOT_ALLOWED, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            await member.ReplaceRolesAsync(Enumerable.Empty<DiscordRole>()).ConfigureAwait(false);
            await ctx.RespondAsync("Removed all roles from " + Formatter.Bold(member.DisplayName))
                .ConfigureAwait(false);
        }

        #endregion COMMAND_REVOKE_ALL

        #region COMMAND_ASSIGN

        [Command("setrole")]
        [Aliases("addrole")]
        [Description("Assign a role to server user.")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task SetUserRole(CommandContext ctx,
            [Description("Server user name.")] DiscordMember member,
            [Description("Server role name.")] [RemainingText]
            DiscordRole role)
        {
            member ??= ctx.Member;
            await member.GrantRoleAsync(role).ConfigureAwait(false);
            await ctx.RespondAsync(member.DisplayName + " been granted the role " + Formatter.Bold(role.Name))
                .ConfigureAwait(false);
        }

        #endregion COMMAND_ASSIGN

        #region COMMAND_SHOW

        [Command("show")]
        [Aliases("display", "hide")]
        [Description("Toggle whether this role is seen or not.")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task SidebarRole(CommandContext ctx,
            [Description("Server role name.")] [RemainingText]
            DiscordRole role)
        {
            if (role is null) return;

            if (role.IsHoisted)
            {
                await role.ModifyAsync(hoist: false).ConfigureAwait(false);
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " is now **hidden**")
                    .ConfigureAwait(false);
            }
            else
            {
                await role.ModifyAsync(hoist: true).ConfigureAwait(false);
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " is now **displayed**")
                    .ConfigureAwait(false);
            }
        }

        #endregion COMMAND_SHOW
    }
}