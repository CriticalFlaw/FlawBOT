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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("server", "Slash command group for modal server commands.")]
    public class ServerModule : ApplicationCommandModule
    {
        /// <summary>
        /// Changes server avatar.
        /// </summary>
        [SlashCommand("avatar", "Change the server avatar.")]
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

        /// <summary>
        /// Returns server information.
        /// </summary>
        [SlashCommand("info", "Returns server information.")]
        public async Task GetServerInfo(InteractionContext ctx)
        {
            var output = new DiscordEmbedBuilder()
                .WithAuthor($"Owner: {ctx.Guild.Owner.Username}#{ctx.Guild.Owner.Discriminator}", iconUrl: ctx.Guild.Owner.AvatarUrl ?? string.Empty)
                .WithTitle(ctx.Guild.Name)
                .WithDescription("ID: " + ctx.Guild.Id)
                .AddField("Created on", ctx.Guild.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Member Count", ctx.Guild.MemberCount.ToString(), true)
                .AddField("Region", ctx.Guild.VoiceRegion.Name.ToUpperInvariant(), true)
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

        /// <summary>
        /// Returns server invite link.
        /// </summary>
        [SlashCommand("invite", "Returns server invite link.")]
        public async Task GetServerInvite(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("Instant Invite to " + Formatter.Bold(ctx.Guild.Name) + ": https://discord.gg/" + ctx.Channel.CreateInviteAsync().Result.Code).ConfigureAwait(false);
        }

        /// <summary>
        /// Prunes inactive users from the server.
        /// </summary>
        //[SlashCommand("prune", "Prunes inactive users from the server.")]
        //[SlashRequirePermissions(Permissions.DeafenMembers)]
        //public async Task PruneUsers(InteractionContext ctx, [Option("days", "Number of days the user had to be inactive to get pruned.")] int days = 7)
        //{
        //    if (days < 1 || days > 30)
        //        await BotServices.SendResponseAsync(ctx, "Number of days must be between 1 and 30", ResponseType.Warning).ConfigureAwait(false);

        //    var count = await ctx.Guild.GetPruneCountAsync(days).ConfigureAwait(false);
        //    if (count == 0)
        //    {
        //        await ctx.CreateResponseAsync("No inactive members found to prune").ConfigureAwait(false);
        //        return;
        //    }

        //    var prompt = await ctx.CreateResponseAsync($"Pruning will remove {Formatter.Bold(count.ToString())} member(s).\nRespond with **yes** to continue.").ConfigureAwait(false);
        //    var interactivity = await BotServices.GetUserInteractivity(ctx, ":heavy_check_mark:", 10).ConfigureAwait(false);
        //    if (interactivity.Result is null) return;
        //    await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
        //    await BotServices.RemoveMessage(prompt).ConfigureAwait(false);
        //    await ctx.Guild.PruneAsync(days).ConfigureAwait(false);
        //}

        /// <summary>
        /// Changes the name of the server.
        /// </summary>
        [SlashCommand("rename", "hanges the name of the server.")]
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