using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("server", "Slash command group for server commands.")]
    public class ServerModule : ApplicationCommandModule
    {
        [SlashCommand("avatar", "Changes the server avatar.")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetServerAvatar(InteractionContext ctx, [Option("query", "Image URL in JPG, PNG or IMG format.")] string query)
        {
            try
            {
                var stream = BotServices.CheckImageInput(ctx, query).Result;
                await ctx.Guild.ModifyAsync(chn => chn.Icon = stream).ConfigureAwait(false);
                await ctx.CreateResponseAsync(ctx.Guild.Name + " server avatar has been updated!").ConfigureAwait(false);
            }
            catch
            {
                await BotServices.SendResponseAsync(ctx, ctx.Guild.Name + " server avatar has not been updated!", ResponseType.Error).ConfigureAwait(false);
            }
        }

        [SlashCommand("info", "Returns server information.")]
        public async Task GetServerInfo(InteractionContext ctx)
        {
            var output = new DiscordEmbedBuilder()
                .WithAuthor($"Owner: {ctx.Guild.Owner.Username}#{ctx.Guild.Owner.Discriminator}", iconUrl: ctx.Guild.Owner.AvatarUrl ?? string.Empty)
                .WithTitle(ctx.Guild.Name)
                .WithDescription("ID: " + ctx.Guild.Id)
                .AddField("Created on", ctx.Guild.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Member Count", ctx.Guild.MemberCount.ToString(), true)
                .AddField("Authentication", ctx.Guild.MfaLevel.ToString(), true)
                .AddField("Content Filter", ctx.Guild.ExplicitContentFilter.ToString(), true)
                .AddField("Verification", ctx.Guild.VerificationLevel.ToString(), true)
                .WithFooter(ctx.Guild.Name + " / #" + ctx.Channel.Name + " / " + DateTime.Now)
                .WithColor(DiscordColor.Rose);

            if (!string.IsNullOrEmpty(ctx.Guild.IconHash))
                output.WithThumbnail(ctx.Guild.IconUrl);

            var roles = new StringBuilder();
            foreach (var role in ctx.Guild.Roles)
                roles.Append($"[`{role.Value.Name}`]");
            if (roles.Length == 0) roles.Append("None");
            output.AddField("Roles", roles.ToString());

            var emojis = new StringBuilder();
            foreach (var emoji in ctx.Guild.Emojis)
                emojis.Append(emoji.Value.Name + (!emoji.Equals(ctx.Guild.Emojis.Last()) ? ", " : string.Empty));
            if (emojis.Length != 0) output.AddField("Emojis", emojis.ToString(), true);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        [SlashCommand("invite", "Returns server invite link.")]
        public async Task GetServerInvite(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("Instant Invite to " + Formatter.Bold(ctx.Guild.Name) + ": https://discord.gg/" + ctx.Channel.CreateInviteAsync().Result.Code).ConfigureAwait(false);
        }

        [SlashCommand("rename", "Changes the name of the server.")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetServerName(InteractionContext ctx, [Option("name", "New server name.")] string name = "")
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            {
                await BotServices.SendResponseAsync(ctx, "Server name cannot be blank or over 100 characters!", ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await ctx.Guild.ModifyAsync(srv => srv.Name = name).ConfigureAwait(false);
            await ctx.CreateResponseAsync("Server name has been changed to " + Formatter.Bold(name)).ConfigureAwait(false);
        }
    }
}