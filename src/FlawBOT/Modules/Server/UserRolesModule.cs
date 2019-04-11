using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Server
{
    [Group("roles")]
    [Description("Miscellaneous role control commands. Group call lists all the roles in this guild or prints information about a given role.")]
    [Aliases("role", "rl")]
    [Cooldown(3, 5, CooldownBucketType.Guild)]
    public class UserRolesModule : BaseCommandModule
    {
        #region COMMAND_COLOR

        [Command("color")]
        [Aliases("clrr")]
        [Description("Set a role's color")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task ColorRole(CommandContext ctx, DiscordRole role, DiscordColor color)
        {
            await role.UpdateAsync(color: color).ConfigureAwait(false);
            var output = new DiscordEmbedBuilder()
                .WithTitle($"Successfully set the color for the role {Formatter.Bold(role.Name)} to {Formatter.InlineCode(role.Color.ToString())}")
                .WithColor(color);
            var message = await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_COLOR

        #region COMMAND_MENTION

        [Command("mention")]
        [Aliases("mr")]
        [Description("Toggle whether this role can be mentioned by others")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task MentionRole(CommandContext ctx, [RemainingText] DiscordRole role)
        {
            if (role == null) return;

            if (role.IsMentionable)
            {
                await role.UpdateAsync(mentionable: false);
                await BotServices.SendEmbedAsync(ctx, $"Role **{role.Name}** is now **not-mentionable**");
            }
            else
            {
                await role.UpdateAsync(mentionable: true);
                await BotServices.SendEmbedAsync(ctx, $"Role **{role.Name}** is now **mentionable**");
            }
        }

        #endregion COMMAND_MENTION

        #region COMMAND_SHOW

        [Command("showrole")]
        [Aliases("dr", "show", "display")]
        [Description("Toggles whether this role is displayed in the sidebar or not")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task SidebarRole(CommandContext ctx, [RemainingText] DiscordRole role)
        {
            if (role == null) return;

            if (role.IsHoisted)
            {
                await role.UpdateAsync(hoist: false);
                await BotServices.SendEmbedAsync(ctx, $"Role {role.Name} is now **hidden**");
            }
            else
            {
                await role.UpdateAsync(hoist: true);
                await BotServices.SendEmbedAsync(ctx, $"Role {role.Name} is now **displayed**");
            }
        }

        #endregion COMMAND_SHOW

        #region COMMAND_INFO

        [Command("info")]
        [Aliases("role")]
        [Description("Retrieve role information")]
        public async Task GetRole(CommandContext ctx, [RemainingText] string roleName)
        {
            var role = ctx.Guild.Roles.FirstOrDefault(r => r.Name.ToLowerInvariant() == roleName);
            if (role != null)
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(role.Name + $" (ID: {role.Id}")
                    .WithDescription($"Created on {role.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}")
                    .AddField("Permissions", role.Permissions.ToPermissionString())
                    .AddField("Managed", role.IsManaged ? "YES" : "NO", true)
                    .AddField("Hoisted", role.IsHoisted ? "YES" : "NO", true)
                    .AddField("Mentionable", role.IsMentionable ? "YES" : "NO", true)
                    .WithThumbnailUrl(ctx.Guild.IconUrl)
                    .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                    .WithColor(role.Color);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_INFO

        #region COMMAND_SETROLE

        [Command("setrole")]
        [Aliases("sr", "addrole")]
        [Description("Set a role for mentioned user")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task SetUserRole(CommandContext ctx, DiscordMember member, [RemainingText] DiscordRole role)
        {
            member = member ?? ctx.Member;
            await member.GrantRoleAsync(role);
            await BotServices.SendEmbedAsync(ctx, member.DisplayName + $" been granted the role **{role.Name}**", EmbedType.Good);
        }

        #endregion COMMAND_SETROLE

        #region COMMAND_REVOKEROLE

        [Command("removerole")]
        [Aliases("rr")]
        [Description("Remove a role from mentioned user")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task RemoveUserRole(CommandContext ctx, DiscordMember member, [RemainingText] DiscordRole role)
        {
            member = member ?? ctx.Member;

            if (role != null)
            {
                await member.RevokeRoleAsync(role);
                await BotServices.SendEmbedAsync(ctx, member.DisplayName + $" has been revoked the role **{role.Name}**", EmbedType.Good);
            }
        }

        #endregion COMMAND_REVOKEROLE

        #region COMMAND_REVOKEROLES

        [Command("removeroles")]
        [Aliases("rrs")]
        [Description("Remove all roles from mentioned user")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task RemoveUserRoles(CommandContext ctx, DiscordMember member)
        {
            if (member.Roles.Max(r => r.Position) >= ctx.Member.Roles.Max(r => r.Position))
                await BotServices.SendEmbedAsync(ctx, ":warning: You are unauthorised to remove roles from this user!", EmbedType.Warning);
            else
            {
                await member.ReplaceRolesAsync(Enumerable.Empty<DiscordRole>()).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, "Removed all roles from " + member.DisplayName, EmbedType.Good);
            }
        }

        #endregion COMMAND_REVOKEROLES

        #region COMMAND_INROLE

        [Command("inrole")]
        [Description("Lists all users in specified role")]
        public async Task UsersInRole(CommandContext ctx, [RemainingText] string roleName)
        {
            var role = ctx.Guild.Roles.FirstOrDefault(r => r.Name.ToLowerInvariant() == roleName);
            if (role != null)
            {
                var userCount = 0;
                var usersList = new StringBuilder();
                var users = (await ctx.Guild.GetAllMembersAsync()).ToArray();
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
                    await BotServices.SendEmbedAsync(ctx, $"Role **{role.Name}** has no members");
                else
                    await BotServices.SendEmbedAsync(ctx, $"Role **{role.Name}** has **{userCount}** member(s): {usersList}");
            }
        }

        #endregion COMMAND_INROLE

        #region COMMAND_CREATEROLE

        [Command("create")]
        [Aliases("new")]
        [Description("Create a server role")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task CreateRole(CommandContext ctx, [RemainingText] string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                await BotServices.SendEmbedAsync(ctx, ":warning: Role name cannot be blank!", EmbedType.Warning);
            else
            {
                await ctx.Guild.CreateRoleAsync(role);
                await BotServices.SendEmbedAsync(ctx, $"Role **{role}** has been **created**", EmbedType.Good);
            }
        }

        #endregion COMMAND_CREATEROLE

        #region COMMAND_DELETEROLE

        [Command("delete")]
        [Aliases("remove")]
        [Description("Delete a server role")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task DeleteRole(CommandContext ctx, [RemainingText] DiscordRole role)
        {
            if (role == null)
                await BotServices.SendEmbedAsync(ctx, ":mag: Role not found in the server!", EmbedType.Warning);
            else
            {
                await role.DeleteAsync();
                await BotServices.SendEmbedAsync(ctx, $"Role **{role.Name}** has been **removed**", EmbedType.Good);
            }
        }

        #endregion COMMAND_DELETEROLE
    }
}