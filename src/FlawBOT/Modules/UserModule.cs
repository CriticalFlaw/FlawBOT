using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using FlawBOT.Common;
using FlawBOT.Services;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("user", "Slash command group for user commands.")]
    [SlashRequireGuild]
    public class UserModule : ApplicationCommandModule
    {
        [SlashCommand("avatar", "Returns server user's profile picture.")]
        public async Task GetAvatar(InteractionContext ctx, [Option("user", "Server user whose profile picture to retrieve.")] DiscordUser user = null)
        {
            user ??= ctx.User;
            var output = new DiscordEmbedBuilder()
                .WithTitle(BotServices.FormatUserName(user))
                .WithImageUrl(user.AvatarUrl)
                .WithColor(DiscordColor.Lilac);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        [SlashCommand("ban", "Bans a server user for a given day duration and reason.")]
        [SlashRequirePermissions(Permissions.BanMembers)]
        public async Task BanUser(InteractionContext ctx,
            [Option("user", "Server user to ban.")] DiscordUser user,
            [Choice("None", 0)][Choice("1 Day", 1)][Choice("1 Week", 7)][Option("deletedays", "Number of days of message history to delete")] long deleteDays = 0,
            [Option("reason", "Reason for the ban.")] string reason = null)
        {
            if (ctx.User.Id == user.Id)
            {
                await BotServices.SendResponseAsync(ctx, "You cannot ban yourself.", ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            if (user.Id == ctx.Client.CurrentUser.Id)
            {
                await BotServices.SendResponseAsync(ctx, "Use `/bot leave` to remove me from the server.", ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await ctx.Guild.BanMemberAsync(user.Id, (int)deleteDays, reason).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{Formatter.Sanitize(user.Username)} has been **banned** from the server.").ConfigureAwait(false);
        }

        [SlashCommand("info", "Returns server user's information.")]
        public async Task GetUser(InteractionContext ctx, [Option("member", "Server user whose information to retrieve.")] DiscordUser user = null)
        {
            user ??= ctx.User;
            var title = "@" + BotServices.FormatUserName(user);
            if (user.IsBot) title += " __[BOT]__";
            if (user.Id == ctx.Guild.OwnerId) title += " __[OWNER]__";

            var output = new DiscordEmbedBuilder()
                .WithTitle(title)
                .WithDescription("ID: " + user.Id)
                .AddSafeField("Registered on", user.CreationTimestamp.UtcDateTime.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture), true)
                .WithThumbnail(user.AvatarUrl)
                .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name}")
                .WithTimestamp(DateTimeOffset.UtcNow)
                .WithColor(user.BannerColor ?? DiscordColor.Black);

            // Presence data is only available with the privileged presence intent.
            if (user.Presence is not null) output.AddSafeField("Presence", user.Presence.Status.ToString(), true);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        [SlashCommand("unban", "Unbans a user from the server.")]
        [SlashRequirePermissions(Permissions.BanMembers)]
        public async Task Unban(InteractionContext ctx, [Option("userid", "Discord user ID to unban from the server.")] string userId, [Option("reason", "Reason for the unban.")] string reason = null)
        {
            // User IDs are too large for Discord's integer options, so they are accepted as text.
            if (!ulong.TryParse(userId?.Trim(), out var id))
            {
                await BotServices.SendResponseAsync(ctx, "Please provide a valid Discord user ID.", ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var user = await ctx.Client.GetUserAsync(id).ConfigureAwait(false);
            await ctx.Guild.UnbanMemberAsync(user, reason ?? "No reason provided.").ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{Formatter.Sanitize(user.Username)} has been **unbanned** from the server.").ConfigureAwait(false);
        }
    }
}
