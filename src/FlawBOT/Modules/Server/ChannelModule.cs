using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;

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
        public async Task CreateCategory(CommandContext ctx,
            [Description("New category name")] [RemainingText]
            string name)
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_NAME, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            var category = await ctx.Guild.CreateChannelCategoryAsync(name.Trim()).ConfigureAwait(false);
            await ctx.RespondAsync("Successfully created category " + Formatter.Bold(category.Name))
                .ConfigureAwait(false);
        }

        #endregion COMMAND_CATEGORY

        #region CHANNEL_CLEAN

        [Command("clean")]
        [Aliases("clear")]
        [Description("Remove channel messages")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task CleanChannel(CommandContext ctx,
            [Description("Number of message to remove from the current channel")]
            int limit = 2)
        {
            var messages = await ctx.Channel.GetMessagesAsync(BotServices.LimitToRange(limit)).ConfigureAwait(false);
            await ctx.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
            await ctx.RespondAsync(Formatter.Bold(messages.Count.ToString()) + " message(s) removed from #" +
                                   ctx.Channel.Name).ConfigureAwait(false);
        }

        #endregion CHANNEL_CLEAN

        #region COMMAND_DELETE

        [Command("delete")]
        [Aliases("remove")]
        [Description("Delete a channel. If a channel isn't specified, the current one will be deleted")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task DeleteChannel(CommandContext ctx,
            [Description("Channel to delete")] [RemainingText]
            DiscordChannel channel = null)
        {
            // Set the current channel for deletion if one isn't provided by the user
            channel ??= ctx.Channel;

            var prompt = await ctx
                .RespondAsync("You're about to delete the " + Formatter.Bold(channel.ToString()) +
                              "\nRespond with **yes** if you want to proceed or wait 10 seconds to cancel the operation.")
                .ConfigureAwait(false);
            var interactivity = await BotServices.GetUserInteractivity(ctx, "yes", 10).ConfigureAwait(false);
            if (interactivity.Result is null)
            {
                await ctx.RespondAsync(Resources.INFO_REQ_TIMEOUT).ConfigureAwait(false);
                return;
            }

            await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            await BotServices.RemoveMessage(prompt).ConfigureAwait(false);
            await ctx.RespondAsync("Successfully deleted " + Formatter.Bold(channel.Name)).ConfigureAwait(false);
            await channel.DeleteAsync().ConfigureAwait(false);
        }

        #endregion COMMAND_DELETE

        #region COMMAND_INFO

        [Command("info")]
        [Aliases("i")]
        [Description("Print channel information. If a channel isn't specified, the current one will be used")]
        public Task GetChannel(CommandContext ctx,
            [Description("Channel to retrieve information from")] [RemainingText]
            DiscordChannel channel = null)
        {
            // Set the current channel for viewing if one isn't provided by the user
            channel ??= ctx.Channel;

            // Check that the user has the permission in the channel to view its information
            if (!ctx.Member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels))
                return BotServices.SendResponseAsync(ctx, "You are not allowed to see this channel!",
                    ResponseType.Warning);

            // Create the base embed message
            var output = new DiscordEmbedBuilder()
                .WithTitle(channel.Name + " (" + channel.Id + ")")
                .WithDescription("Topic: " + (channel.IsCategory ? "N/A" : channel.Topic ?? string.Empty))
                .AddField("Type", channel.Type.ToString(), true)
                .AddField("Private", channel.IsPrivate ? "Yes" : "No", true)
                .AddField("NSFW", channel.IsNSFW ? "Yes" : "No", true)
                .WithThumbnail(ctx.Guild.IconUrl)
                .WithFooter("Created on " + channel.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture))
                .WithColor(SharedData.DefaultColor);

            // Add additional fields depending on the channel type
            switch (channel.Type)
            {
                case ChannelType.Voice:
                    output.AddField("Bitrate", channel.Bitrate.ToString(), true);
                    output.AddField("User limit", channel.UserLimit > 0 ? channel.UserLimit.ToString() : "No limit.",
                        true);
                    break;

                case ChannelType.Category:
                    var channels = new StringBuilder();
                    foreach (var chn in channel.Children)
                        channels.Append($"[`{chn.Name}`]");
                    output.AddField("Channels", channels.Length > 0 ? channels.ToString() : "None", true);
                    break;
            }

            return ctx.RespondAsync(output.Build());
        }

        #endregion COMMAND_INFO

        #region CHANNEL_PURGE

        [Command("purge")]
        [Description("Remove server user's channel messages")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task Purge(CommandContext ctx,
            [Description("Server user whose messages will be purged")]
            DiscordMember member,
            [Description("Number of messages to purge")] [RemainingText]
            int limit = 0)
        {
            var messages = await ctx.Channel.GetMessagesAsync(BotServices.LimitToRange(limit)).ConfigureAwait(false);
            await ctx.Channel.DeleteMessagesAsync(messages.Where(m => m.Author.Id == member.Id)).ConfigureAwait(false);
            await ctx.RespondAsync(
                    $"Purged **{limit}** messages by {member.Username}#{member.Discriminator} (ID:{member.Id})")
                .ConfigureAwait(false);
        }

        #endregion CHANNEL_PURGE

        #region COMMAND_RENAME

        [Command("rename")]
        [Aliases("setname")]
        [Description("Rename a channel. If a channel isn't specified, the current one will be used")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task SetChannelName(CommandContext ctx,
            [Description("Channel to rename")] DiscordChannel channel,
            [Description("New channel name")] [RemainingText]
            string name)
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_NAME, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            var oldName = channel.Name;
            await channel.ModifyAsync(m => m.Name = name.Trim().Replace(" ", "-")).ConfigureAwait(false);
            await ctx.RespondAsync("Successfully renamed the channel " + Formatter.Bold(oldName) + " to " +
                                   Formatter.Bold(name)).ConfigureAwait(false);
        }

        #endregion COMMAND_RENAME

        #region COMMAND_TEXT

        [Command("text")]
        [Aliases("createtext", "newtext", "ctc")]
        [Description("Create a new text channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task CreateText(CommandContext ctx,
            [Description("New text channel name")] [RemainingText]
            string name = "")
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_NAME, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            if (ctx.Guild.Channels.Any(chn => string.Equals(name, chn.Value.Name, StringComparison.OrdinalIgnoreCase)))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_EXISTS, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            var channel = await ctx.Guild.CreateTextChannelAsync(name.Trim().Replace(" ", "-"))
                .ConfigureAwait(false);
            await ctx.RespondAsync("Successfully created the text channel " + Formatter.Bold(channel.Name))
                .ConfigureAwait(false);
        }

        #endregion COMMAND_TEXT

        #region COMMAND_TOPIC

        [Command("topic")]
        [Aliases("settopic", "st")]
        [Description("Set current channel's topic")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task SetChannelTopic(CommandContext ctx,
            [Description("New channel topic")] [RemainingText]
            string topic = "")
        {
            if (topic.Length > 1024)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_TOPIC, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            await ctx.Channel.ModifyAsync(chn => chn.Topic = topic).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(topic))
                await ctx.RespondAsync("Successfully changed the channel topic to " + Formatter.Bold(topic))
                    .ConfigureAwait(false);
        }

        #endregion COMMAND_TOPIC

        #region COMMAND_VOICE

        [Command("voice")]
        [Aliases("createvoice", "newvoice", "cvc")]
        [Description("Create a new voice channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task CreateVoice(CommandContext ctx,
            [Description("New voice channel name")] [RemainingText]
            string name = "")
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_NAME, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            if (ctx.Guild.Channels.Any(chn => string.Equals(name, chn.Value.Name, StringComparison.OrdinalIgnoreCase)))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_EXISTS, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            var channel = await ctx.Guild.CreateVoiceChannelAsync(name.Trim().Replace(" ", "-"))
                .ConfigureAwait(false);
            await ctx.RespondAsync("Successfully created the voice channel " + Formatter.Bold(channel.Name))
                .ConfigureAwait(false);
        }

        #endregion COMMAND_VOICE
    }
}