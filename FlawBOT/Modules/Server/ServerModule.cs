using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Models;
using FlawBOT.Services;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Server
{
    [Group("server")]
    [Description("Server control commands.")]
    [Aliases("guild")]
    [Cooldown(3, 5, CooldownBucketType.Guild)]
    public class ServerModule : BaseCommandModule
    {
        #region COMMAND_INVITE

        [Command("invite")]
        [Aliases("inv")]
        [Description("Create an instant invite link for this server")]
        public async Task InviteAsync(CommandContext ctx)
        {
            await BotServices.SendEmbedAsync(ctx, $"Instant Invite to **{ctx.Guild.Name}**: https://discord.gg/{ctx.Channel.CreateInviteAsync().Result.Code}", EmbedType.Warning);
        }

        #endregion COMMAND_INVITE

        #region COMMAND_PRUNE

        [Command("prune")]
        [Description("Prune inactive server members")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task PruneUsers(CommandContext ctx, [RemainingText] string day)
        {
            day = day ?? "30";
            if (int.TryParse(day, out var days))
            {
                await BotServices.SendEmbedAsync(ctx, $"Pruned **{ctx.Guild.GetPruneCountAsync(days).Result}** server members who have been inactive for {day} days.", EmbedType.Good);
                await ctx.Guild.PruneAsync(days);
            }
            else
                await BotServices.SendEmbedAsync(ctx, ":warning: Invalid number of days, try **.prune 30**", EmbedType.Warning);
        }

        #endregion COMMAND_PRUNE

        #region COMMAND_LEAVE

        [Command("leave")]
        [Description("Makes this bot leave the current server.")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task LeaveAsync(CommandContext ctx)
        {
            await BotServices.SendEmbedAsync(ctx, "Are you sure you want FlawBOT to leave this server?\nRespond with **yes** to proceed or wait 10 seconds to cancel this operation.");

            var interactivity = await ctx.Client.GetInteractivity()
                .WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
            if (interactivity == null)
                await BotServices.SendEmbedAsync(ctx, "Request cancelled...");
            else
            {
                await BotServices.SendEmbedAsync(ctx, "Thank you for using FlawBOT...");
                await ctx.Guild.LeaveAsync();
            }
        }

        #endregion COMMAND_LEAVE

        #region COMMAND_PERMS

        [Command("perms")]
        [Aliases("prm")]
        [Description("Retrieve your server permissions")]
        public async Task ListServerPermissions(CommandContext ctx, DiscordMember member = null, DiscordChannel channel = null)
        {
            member = member ?? ctx.Member;
            channel = channel ?? ctx.Channel;
            var perms = $"{Formatter.Bold(member.DisplayName)} cannot access channel {Formatter.Bold(channel.Name)}.";
            if (member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels))
                perms = member.PermissionsIn(channel).ToPermissionString();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"Permissions for {member.Username} in #{channel.Name}:")
                .WithDescription(perms)
                .WithColor(DiscordColor.Turquoise);
            await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_PERMS

        #region COMMAND_INFO

        [Command("info")]
        [Description("Retrieve server information")]
        [Aliases("i")]
        public async Task GetServer(CommandContext ctx)
        {
            var channels = new StringBuilder();
            var roles = new StringBuilder();
            var emojis = new StringBuilder();
            var output = new DiscordEmbedBuilder()
                .WithAuthor($"Owner: {ctx.Guild.Owner.Username}#{ctx.Guild.Owner.Discriminator}", icon_url: string.IsNullOrEmpty(ctx.Guild.Owner.AvatarHash) ? null : ctx.Guild.Owner.AvatarUrl)
                .WithTitle($"{ctx.Guild.Name}")
                .AddField("ID", ctx.Guild.Id.ToString(), true)
                .AddField("Created on", ctx.Guild.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                .WithColor(DiscordColor.Rose);
            if (!string.IsNullOrEmpty(ctx.Guild.IconHash))
                output.WithThumbnailUrl(ctx.Guild.IconUrl);

            foreach (var channel in ctx.Guild.Channels)
                switch (channel.Type)
                {
                    default:
                        channels.Append($"`\n[{channel.Name}]`\n");
                        break;

                    case ChannelType.Text:
                        channels.Append($"`[#{channel.Name}]`");
                        break;

                    case ChannelType.Voice:
                        channels.Append($"`[{channel.Name}]`");
                        break;

                    case ChannelType.Category:
                        channels.Append($"`\n[{channel.Name.ToUpperInvariant()}]`\n");
                        break;

                    case ChannelType.Private:
                    case ChannelType.Group:
                    case ChannelType.Unknown:
                        break;
                }
            if (channels.Length == 0) channels.Append("None");
            output.AddField("Channels", channels.ToString());
            foreach (var role in ctx.Guild.Roles)
                roles.Append($"[`{role.Name}`]");
            foreach (var emoji in ctx.Guild.Emojis)
                emojis.Append(emoji.Name);
            if (roles.Length == 0) roles.Append("None");
            output.AddField("Roles", roles.ToString());
            output.AddField("Member Count", ctx.Guild.MemberCount.ToString(), true);
            output.AddField("Region", ctx.Guild.VoiceRegion.Name.ToUpperInvariant(), true);
            output.AddField("Authentication", ctx.Guild.MfaLevel.ToString(), true);
            output.AddField("Content Filter", ctx.Guild.ExplicitContentFilter.ToString(), true);
            output.AddField("Verification", ctx.Guild.VerificationLevel.ToString(), true);
            if (emojis.Length != 0) output.AddField("Emojis", emojis.ToString(), true);
            await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_INFO

        #region COMMAND_AVATAR

        [Command("avatar")]
        [Aliases("setavatar")]
        [Description("Set server avatar")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetServerAvatar(CommandContext ctx, string query)
        {
            var stream = BotServices.CheckImageInput(ctx, query).Result;
            try
            {
                await ctx.Guild.ModifyAsync(chn => chn.Icon = stream);
                await BotServices.SendEmbedAsync(ctx, ctx.Guild.Name + " server avatar has been updated!", EmbedType.Good);
            }
            catch
            {
                await BotServices.SendEmbedAsync(ctx, ctx.Guild.Name + " server avatar has not been updated!", EmbedType.Error);
            }
        }

        #endregion COMMAND_AVATAR

        #region COMMAND_RENAME

        [Command("setname")]
        [Aliases("rename")]
        [Description("Set server name")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetServerName(CommandContext ctx, [RemainingText] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                await BotServices.SendEmbedAsync(ctx, ":warning: Server name cannot be blank!", EmbedType.Warning);
            else
            {
                await ctx.Guild.ModifyAsync(srv => srv.Name = $"{name}");
                await BotServices.SendEmbedAsync(ctx, $"Server name has been changed to **{name}**", EmbedType.Good);
            }
        }

        #endregion COMMAND_RENAME

        #region COMMAND_WARN

        [Command("warn")]
        [Aliases("w")]
        [Description("Direct message user with a warning")]
        public async Task Warn(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            var output = new DiscordEmbedBuilder()
                .WithTitle("Warning received!")
                .WithDescription($"Server **{ctx.Guild.Name}** issued you a warning!")
                .WithTimestamp(DateTime.Now)
                .WithColor(DiscordColor.Red);
            if (!string.IsNullOrWhiteSpace(reason)) output.AddField("Warning message:", reason);
            output.AddField("Sender:", $"{ctx.Member.Username}#{ctx.Member.Discriminator}");
            var dm = await member.CreateDmChannelAsync().ConfigureAwait(false);
            if (dm == null)
                await BotServices.SendEmbedAsync(ctx, "Unable to direct message this user", EmbedType.Warning);
            else
            {
                await dm.SendMessageAsync(embed: output.Build()).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, $"Successfully sent a warning to {Formatter.Bold(member.Username)}.", EmbedType.Good).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_WARN
    }
}