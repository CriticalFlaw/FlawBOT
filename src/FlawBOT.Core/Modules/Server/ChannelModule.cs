using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Net.Models;
using FlawBOT.Common;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Group("channel")]
    [Aliases("chn", "ch", "c")]
    [Description("Commands for controlling channels")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class ChannelModule : BaseCommandModule
    {
        #region COMMAND_CATEGORY

        [Command("category")]
        [Aliases("createcategory", "newcategory", "ct")]
        [Description("Create a new channel category")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task CreateChannelCategory(CommandContext ctx,
            [Description("New category name")] [RemainingText] string name)
        {
            if (!BotServices.CheckChannelName(name))
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_CHANNEL_NAME, EmbedType.Warning).ConfigureAwait(false);
            else
            {
                var category = await ctx.Guild.CreateChannelCategoryAsync(name.Trim()).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, "Successfully created category " + Formatter.Bold(category.Name), EmbedType.Good).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_CATEGORY

        #region CHANNEL_CLEAN

        [Command("clean")]
        [Aliases("clear")]
        [Description("Remove channel messages")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task Clean(CommandContext ctx,
            [Description("Number of message to remove from the current channel")] int limit = 2)
        {
            var messages = await ctx.Channel.GetMessagesAsync(BotServices.LimitToRange(limit)).ConfigureAwait(false);
            await ctx.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
            await BotServices.SendEmbedAsync(ctx, Formatter.Bold(messages.Count.ToString()) + " message(s) removed from #" + ctx.Channel.Name, EmbedType.Good).ConfigureAwait(false);
        }

        #endregion CHANNEL_CLEAN

        #region COMMAND_DELETE

        [Command("delete")]
        [Aliases("remove")]
        [Description("Delete a channel. If a channel isn't specified, the current one will be deleted")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task RemoveTextChannel(CommandContext ctx,
            [Description("Channel to delete")] [RemainingText] DiscordChannel channel = null)
        {
            // Set the current channel for deletion if one isn't provided by the user
            channel = channel ?? ctx.Channel;

            var prompt = await ctx.RespondAsync("You're about to delete the " + Formatter.Bold(channel.ToString()) + "\nRespond with **yes** if you want to proceed or wait 10 seconds to cancel the operation.").ConfigureAwait(false);
            var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "yes", TimeSpan.FromSeconds(10)).ConfigureAwait(false);
            if (interactivity.Result is null)
                await BotServices.SendEmbedAsync(ctx, Resources.REQUEST_TIMEOUT).ConfigureAwait(false);
            else
            {
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                await BotServices.RemoveMessage(prompt).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, "Successfully deleted " + Formatter.Bold(channel.Name), EmbedType.Good).ConfigureAwait(false);
                await channel.DeleteAsync().ConfigureAwait(false);
            }
        }

        #endregion COMMAND_DELETE

        #region COMMAND_INFO

        [Command("info")]
        [Aliases("i")]
        [Description("Print channel information. If a channel isn't specified, the current one will be used")]
        public Task GetChannel(CommandContext ctx,
            [Description("Channel to retrieve information from")] [RemainingText] DiscordChannel channel = null)
        {
            // Set the current channel for viewing if one isn't provided by the user
            channel = channel ?? ctx.Channel;

            // Check that the user has the permission in the channel to view its information
            if (!ctx.Member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels))
                return BotServices.SendEmbedAsync(ctx, "You are not allowed to see this channel!", EmbedType.Warning);

            // Create the base embed message
            var output = new DiscordEmbedBuilder()
                .WithTitle(channel.Name + $" (ID: {channel.Id})")
                .WithDescription("Channel topic: " + Formatter.Italic(channel.Topic) ?? "")
                .AddField("Type", channel.Type.ToString(), true)
                .AddField("Private", channel.IsPrivate ? "YES" : "NO", true)
                .AddField("NSFW", channel.IsNSFW ? "YES" : "NO", true)
                .WithThumbnailUrl(ctx.Guild.IconUrl)
                .WithFooter("Created on " + channel.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture))
                .WithColor(SharedData.DefaultColor);

            // Add additional fields depending on the channel type
            switch (channel.Type)
            {
                case ChannelType.Voice:
                    output.AddField("Bitrate", channel.Bitrate.ToString() ?? "Unknown", true);
                    output.AddField("User limit", (channel.UserLimit > 0) ? channel.UserLimit.ToString() : "No limit.", true);
                    break;

                case ChannelType.Category:
                    var channels = new StringBuilder();
                    foreach (var chn in channel.Children)
                        channels.Append($"[`{chn.Name}`]");
                    output.AddField("Channels", (channels.Length > 0) ? channels.ToString() : "None", true);
                    break;

                default:
                    break;
            }
            return ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_INFO

        #region COMMAND_JOIN

        [Command("join")]
        [Aliases("j")]
        [Description("Be placed into a specified voice channel")]
        public async Task JoinVoiceChannel(CommandContext ctx,
            [Description("Name of voice channel to join")] [RemainingText] DiscordChannel channel = null)
        {
            if (channel.Type == ChannelType.Voice)
                await ctx.Member.PlaceInAsync(channel).ConfigureAwait(false);
        }

        #endregion COMMAND_JOIN

        #region CHANNEL_PURGE

        [Command("purge")]
        [Description("Remove server user's channel messages")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task Purge(CommandContext ctx,
            [Description("Server user whose messages will be purged")] DiscordMember member,
            [Description("Number of messages to purge")] [RemainingText] int limit = 0)
        {
            var messages = await ctx.Channel.GetMessagesAsync(BotServices.LimitToRange(limit)).ConfigureAwait(false);
            await ctx.Channel.DeleteMessagesAsync(messages.Where(m => m.Author.Id == member.Id)).ConfigureAwait(false);
            await BotServices.SendEmbedAsync(ctx, $"Purged **{limit}** messages by {member.Username}#{member.Discriminator} (ID:{member.Id})", EmbedType.Good).ConfigureAwait(false);
        }

        #endregion CHANNEL_PURGE

        #region COMMAND_RENAME

        [Command("rename")]
        [Aliases("setname")]
        [Description("Rename a channel. If a channel isn't specified, the current one will be used")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task SetChannelName(CommandContext ctx,
            [Description("Channel to rename")] DiscordChannel channel,
            [Description("New channel name")] [RemainingText] string name)
        {
            if (!BotServices.CheckChannelName(name))
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_CHANNEL_NAME, EmbedType.Warning).ConfigureAwait(false);
            else
            {
                var old_name = channel.Name;
                await channel.ModifyAsync(new Action<ChannelEditModel>(m => m.Name = name.Trim().Replace(" ", "-"))).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, $"Successfully renamed the channel " + Formatter.Bold(old_name) + " to " + Formatter.Bold(name), EmbedType.Good).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_RENAME

        #region COMMAND_TEXT

        [Command("text")]
        [Aliases("createtext", "newtext", "ctc")]
        [Description("Create a new text channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task CreateTextChannel(CommandContext ctx,
            [Description("New text channel name")] [RemainingText] string name = "")
        {
            if (!BotServices.CheckChannelName(name))
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_CHANNEL_NAME, EmbedType.Warning).ConfigureAwait(false);
            else if (ctx.Guild.Channels.Any(chn => string.Compare(name, chn.Value.Name, true) == 0))
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_CHANNEL_EXISTS, EmbedType.Warning).ConfigureAwait(false);
            else
            {
                var channel = await ctx.Guild.CreateTextChannelAsync(name.Trim().Replace(" ", "-")).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, "Successfully created the text channel " + Formatter.Bold(channel.Name), EmbedType.Good).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_TEXT

        #region COMMAND_TOPIC

        [Command("topic")]
        [Aliases("settopic", "st")]
        [Description("Set current channel's topic")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task SetChannelTopic(CommandContext ctx,
            [Description("New channel topic")] [RemainingText] string topic = "")
        {
            if (topic.Length > 1024)
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_CHANNEL_TOPIC, EmbedType.Warning).ConfigureAwait(false);
            else
            {
                await ctx.Channel.ModifyAsync(chn => chn.Topic = topic).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(topic))
                    await BotServices.SendEmbedAsync(ctx, "Successfully changed the channel topic to " + Formatter.Bold(topic), EmbedType.Good).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_TOPIC

        #region COMMAND_VOICE

        [Command("voice")]
        [Aliases("createvoice", "newvoice", "cvc")]
        [Description("Create a new voice channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task CreateVoiceChannel(CommandContext ctx,
            [Description("New voice channel name")] [RemainingText] string name = "")
        {
            if (!BotServices.CheckChannelName(name))
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_CHANNEL_NAME, EmbedType.Warning).ConfigureAwait(false);
            else if (ctx.Guild.Channels.Any(chn => string.Compare(name, chn.Value.Name, true) == 0))
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_CHANNEL_EXISTS, EmbedType.Warning).ConfigureAwait(false);
            else
            {
                var channel = await ctx.Guild.CreateVoiceChannelAsync(name: name.Trim().Replace(" ", "-")).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, "Successfully created the voice channel " + Formatter.Bold(channel.Name), EmbedType.Good).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_VOICE
    }
}