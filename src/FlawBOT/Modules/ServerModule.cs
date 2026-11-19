using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("server", "Slash command group for server commands.")]
    [SlashRequireGuild]
    public class ServerModule : ApplicationCommandModule
    {
        [SlashCommand("avatar", "Changes the server avatar.")]
        [SlashRequirePermissions(Permissions.ManageGuild)]
        public async Task SetServerAvatar(InteractionContext ctx, [Option("query", "Image URL in JPG, PNG or GIF format.")] string query)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            await using var stream = await BotServices.CheckImageInput(query).ConfigureAwait(false);
            if (stream is null)
            {
                await BotServices.SendDeferredAsync(ctx, Resources.URL_INVALID_IMG, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await ctx.Guild.ModifyAsync(x => x.Icon = stream).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, ctx.Guild.Name + " server avatar has been updated!").ConfigureAwait(false);
        }

        [SlashCommand("info", "Returns server information.")]
        public async Task GetServerInfo(InteractionContext ctx)
        {
            var guild = ctx.Guild;
            // The owner may not be cached without the members intent, so fetch it.
            var owner = await guild.GetMemberAsync(guild.OwnerId).ConfigureAwait(false);
            var output = new DiscordEmbedBuilder()
                .WithAuthor($"Owner: {BotServices.FormatUserName(owner)}", iconUrl: owner?.AvatarUrl)
                .WithTitle(guild.Name)
                .WithDescription("ID: " + guild.Id)
                .AddSafeField("Created on", guild.CreationTimestamp.UtcDateTime.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture), true)
                .AddSafeField("Member Count", guild.MemberCount.ToString(), true)
                .AddSafeField("Authentication", guild.MfaLevel.ToString(), true)
                .AddSafeField("Content Filter", guild.ExplicitContentFilter.ToString(), true)
                .AddSafeField("Verification", guild.VerificationLevel.ToString(), true)
                .AddSafeField("Roles", guild.Roles.Values.OrderByDescending(x => x.Position).Select(x => $"[`{x.Name}`]").JoinWithinLimit(""))
                .WithFooter($"{guild.Name} / #{ctx.Channel.Name}")
                .WithTimestamp(DateTimeOffset.UtcNow)
                .WithColor(DiscordColor.Rose);
            if (!string.IsNullOrEmpty(guild.IconHash))
                output.WithThumbnail(guild.IconUrl);
            if (guild.Emojis.Count > 0)
                output.AddSafeField("Emojis", guild.Emojis.Values.Select(x => x.Name).JoinWithinLimit(", "), true);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        [SlashCommand("invite", "Returns server invite link.")]
        [SlashRequireBotPermissions(Permissions.CreateInstantInvite)]
        public async Task GetServerInvite(InteractionContext ctx)
        {
            var invite = await ctx.Channel.CreateInviteAsync().ConfigureAwait(false);
            await ctx.CreateResponseAsync("Instant Invite to " + Formatter.Bold(ctx.Guild.Name) + ": https://discord.gg/" + invite.Code).ConfigureAwait(false);
        }

        [SlashCommand("rename", "Changes the name of the server.")]
        [SlashRequirePermissions(Permissions.ManageGuild)]
        public async Task SetServerName(InteractionContext ctx, [Option("name", "New server name.")] string name = "")
        {
            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 2 || name.Trim().Length > 100)
            {
                await BotServices.SendResponseAsync(ctx, "Server name must be between 2 and 100 characters!", ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await ctx.Guild.ModifyAsync(srv => srv.Name = name.Trim()).ConfigureAwait(false);
            await ctx.CreateResponseAsync("Server name has been changed to " + Formatter.Bold(name.Trim())).ConfigureAwait(false);
        }
    }
}
