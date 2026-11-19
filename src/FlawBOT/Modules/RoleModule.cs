using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("role", "Slash command group for role commands.")]
    [SlashRequireGuild]
    public class RoleModule : ApplicationCommandModule
    {
        [SlashCommand("create", "Creates a new server role.")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task CreateRole(InteractionContext ctx, [Option("role", "New role name.")] string role = "")
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_NAME, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await ctx.Guild.CreateRoleAsync(role.Trim()).ConfigureAwait(false);
            await ctx.CreateResponseAsync("Successfully created the server role " + Formatter.Bold(role.Trim())).ConfigureAwait(false);
        }

        [SlashCommand("delete", "Deletes a server role.")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task DeleteRole(InteractionContext ctx, [Option("role", "Server role to remove.")] DiscordRole role)
        {
            if (role is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_EXISTING, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await role.DeleteAsync().ConfigureAwait(false);
            await ctx.CreateResponseAsync("Successfully removed the server role " + Formatter.Bold(role.Name)).ConfigureAwait(false);
        }

        [SlashCommand("info", "Returns server role information.")]
        public async Task GetRoleInfo(InteractionContext ctx, [Option("role", "Server role name.")] DiscordRole role)
        {
            if (role is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_EXISTING, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var output = new DiscordEmbedBuilder()
                .WithTitle(role.Name)
                .WithDescription("ID: " + role.Id)
                .AddSafeField("Creation Date", role.CreationTimestamp.UtcDateTime.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture), true)
                .AddSafeField("Hoisted", role.IsHoisted ? ":heavy_check_mark:" : ":x:", true)
                .AddSafeField("Mentionable", role.IsMentionable ? ":heavy_check_mark:" : ":x:", true)
                .AddSafeField("Permissions", role.Permissions.ToPermissionString())
                .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name}")
                .WithTimestamp(System.DateTimeOffset.UtcNow)
                .WithColor(role.Color);
            if (!string.IsNullOrEmpty(ctx.Guild.IconUrl)) output.WithThumbnail(ctx.Guild.IconUrl);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        [SlashCommand("inrole", "Returns list of users in a given role.")]
        public async Task GetRoleUsers(InteractionContext ctx, [Option("role", "Server role name.")] DiscordRole role)
        {
            if (role is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_EXISTING, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            // Fetching every member can take a while on larger servers.
            // Listing members requires the "Server Members Intent" to be enabled in the Discord developer portal.
            await ctx.DeferAsync().ConfigureAwait(false);
            var members = (await ctx.Guild.GetAllMembersAsync().ConfigureAwait(false))
                .Where(x => x.Roles.Any(r => r.Id == role.Id))
                .Select(x => x.DisplayName)
                .OrderBy(x => x)
                .ToList();

            if (members.Count == 0)
            {
                await BotServices.SendDeferredAsync(ctx, Formatter.Bold(role.Name) + " has no members").ConfigureAwait(false);
                return;
            }

            var header = $"{Formatter.Bold(role.Name)} has {Formatter.Bold(members.Count.ToString())} member(s): ";
            var list = members.JoinWithinLimit(", ", EmbedExtensions.MessageLimit - header.Length - 20);
            if (list.Length < string.Join(", ", members).Length) list += ", ...";
            await BotServices.SendDeferredAsync(ctx, header + list).ConfigureAwait(false);
        }

        [SlashCommand("mention", "Toggles whether the role can be mentioned by others.")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task SetRoleMention(InteractionContext ctx, [Option("role", "Server role name.")] DiscordRole role)
        {
            if (role is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_EXISTING, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var mentionable = !role.IsMentionable;
            await role.ModifyAsync(mentionable: mentionable).ConfigureAwait(false);
            await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " is now " + Formatter.Bold(mentionable ? "mentionable" : "not-mentionable")).ConfigureAwait(false);
        }
    }
}
