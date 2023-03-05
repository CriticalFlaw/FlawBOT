using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("role", "Slash command group for modal role commands.")]
    public class RoleModule : ApplicationCommandModule
    {
        [SlashCommand("color", "Change the server role color.")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task ColorRole(CommandContext ctx, [Option("color", "Server role's new HEX color code.")] DiscordColor color, [Option("role", "Server role to recolor.")] DiscordRole role)
        {
            var regex = new Regex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", RegexOptions.Compiled).Match(color.ToString());
            if (!regex.Success)
            {
                await BotServices.SendResponseAsync(ctx, "Invalid color code. Please enter a HEX color code like #E7B53B", ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await role.ModifyAsync(color: color).ConfigureAwait(false);
            var output = new DiscordEmbedBuilder()
                .WithTitle("Successfully set the color for the role " + Formatter.Bold(role.Name) + " to " + Formatter.InlineCode(role.Color.ToString()))
                .WithColor(color);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        [SlashCommand("create", "Create a new server role.")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task CreateRole(CommandContext ctx, [Option("role", "New role name.")] string role = "")
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_NAME, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await ctx.Guild.CreateRoleAsync(role).ConfigureAwait(false);
            await ctx.RespondAsync("Successfully created the server role " + Formatter.Bold(role)).ConfigureAwait(false);
        }

        [SlashCommand("delete", "Delete a server role..")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task DeleteRole(CommandContext ctx, [Option("role", "Server role to remove.")] DiscordRole role = null)
        {
            if (role is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_EXISTING, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await role.DeleteAsync().ConfigureAwait(false);
            await ctx.RespondAsync("Successfully removed the server role " + Formatter.Bold(role.Name)).ConfigureAwait(false);
        }

        [SlashCommand("info", "Retrieve server role information.")]
        public async Task GetRole(CommandContext ctx, [Option("role", "Server role name.")] DiscordRole role = null)
        {
            if (role is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_EXISTING, ResponseType.Warning).ConfigureAwait(false);
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

        [SlashCommand("inrole", "Retrieve a list of users with a given role.")]
        public async Task UsersInRole(CommandContext ctx, [Option("role", "Server role name.")] DiscordRole role  = null)
        {
            if (role is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_EXISTING, ResponseType.Warning).ConfigureAwait(false);
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
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " has no members").ConfigureAwait(false);
            else
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + $" has **{userCount}** member(s): {usersList}").ConfigureAwait(false);
        }

        [SlashCommand("mention", "Toggle whether this role can be mentioned by others.")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task MentionRole(CommandContext ctx, [Option("role", "Server role name.")] DiscordRole role)
        {
            if (role is null) return;
            if (role.IsMentionable)
            {
                await role.ModifyAsync(mentionable: false).ConfigureAwait(false);
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " is now " + Formatter.Bold("not-mentionable")).ConfigureAwait(false);
            }
            else
            {
                await role.ModifyAsync(mentionable: true).ConfigureAwait(false);
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " is now " + Formatter.Bold("mentionable")).ConfigureAwait(false);
            }
        }

        [SlashCommand("revoke", "Remove a role from server user.")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task RevokeRole(CommandContext ctx, [Option("member", "Server user to get revoked.")] DiscordMember member, [Option("role", "Server role name.")] DiscordRole role)
        {
            if (role != null)
            {
                member ??= ctx.Member;
                await member.RevokeRoleAsync(role).ConfigureAwait(false);
                await ctx.RespondAsync(Formatter.Bold(member.DisplayName) + " has been removed from the role " + Formatter.Bold(role.Name)).ConfigureAwait(false);
            }
        }

        [SlashCommand("revokeall", "Remove all role from a server user.")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task RevokeAllRoles(CommandContext ctx, [Option("member", "Server user name.")] DiscordMember member)
        {
            if (!member.Roles.Any())
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_NONE, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            if (member.Roles.Max(r => r.Position) >= ctx.Member.Roles.Max(r => r.Position))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_NOT_ALLOWED, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await member.ReplaceRolesAsync(Enumerable.Empty<DiscordRole>()).ConfigureAwait(false);
            await ctx.RespondAsync("Removed all roles from " + Formatter.Bold(member.DisplayName)).ConfigureAwait(false);
        }

        [SlashCommand("setrole", "Assign a role to server user.")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task SetUserRole(CommandContext ctx, [Option("member", "Server user name.")] DiscordMember member, [Option("role", "Server role name.")] DiscordRole role)
        {
            member ??= ctx.Member;
            await member.GrantRoleAsync(role).ConfigureAwait(false);
            await ctx.RespondAsync(member.DisplayName + " been granted the role " + Formatter.Bold(role.Name)).ConfigureAwait(false);
        }

        [SlashCommand("show", "Toggle whether this role is seen or not.")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task SidebarRole(CommandContext ctx, [Option("role", "Server role name.")] DiscordRole role)
        {
            if (role is null) return;

            if (role.IsHoisted)
            {
                await role.ModifyAsync(hoist: false).ConfigureAwait(false);
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " is now " + Formatter.Bold("hidden")).ConfigureAwait(false);
            }
            else
            {
                await role.ModifyAsync(hoist: true).ConfigureAwait(false);
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " is now " + Formatter.Bold("displayed")).ConfigureAwait(false);
            }
        }
    }
}