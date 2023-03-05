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
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("channel", "Slash command group for modal channel commands.")]
    public class ChannelModule : ApplicationCommandModule
    {
        [SlashCommand("category_new", "Create a new channel category.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task CreateCategory(InteractionContext ctx, [Option("query", "New category name.")] string name)
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_NAME, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var category = await ctx.Guild.CreateChannelCategoryAsync(name.Trim()).ConfigureAwait(false);
            await ctx.CreateResponseAsync("Successfully created category " + Formatter.Bold(category.Name)).ConfigureAwait(false);
        }

        [SlashCommand("clean", "Remove channel messages.")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task CleanChannel(InteractionContext ctx, [Option("query", "Number of messages to remove from the current channel.")] double limit = 2)
        {
            var messages = await ctx.Channel.GetMessagesAsync(BotServices.LimitToRange(limit)).ConfigureAwait(false);
            await ctx.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
            await ctx.CreateResponseAsync(Formatter.Bold(messages.Count.ToString()) + " message(s) removed from #" + ctx.Channel.Name).ConfigureAwait(false);
        }

        [SlashCommand("delete", "Delete a channel. If a channel isn't specified, the current one will be deleted.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task DeleteChannel(InteractionContext ctx, [Option("query", "Channel to delete.")] DiscordChannel channel = null)
        {
            // Set the current channel for deletion if one isn't provided by the user
            channel ??= ctx.Channel;

            await ctx.CreateResponseAsync("You're about to delete the " + Formatter.Bold(channel.ToString()) + "\nRespond with **yes** if you want to proceed or wait 10 seconds to cancel the operation.").ConfigureAwait(false);
            var interactivity = await BotServices.GetUserInteractivity(ctx, "yes", 10).ConfigureAwait(false);
            if (interactivity.Result is null)
            {
                await ctx.CreateResponseAsync(Resources.INFO_REQ_TIMEOUT).ConfigureAwait(false);
                return;
            }

            await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            await ctx.CreateResponseAsync("Successfully deleted " + Formatter.Bold(channel.Name)).ConfigureAwait(false);
            await channel.DeleteAsync().ConfigureAwait(false);
        }

        [SlashCommand("info", "Print channel information. If a channel isn't specified, the current one will be used.")]
        public Task GetChannel(InteractionContext ctx, [Option("query", "Channel to retrieve information from.")] DiscordChannel channel = null)
        {
            // Set the current channel for viewing if one isn't provided by the user
            channel ??= ctx.Channel;

            // Check that the user has the permission in the channel to view its information
            if (!ctx.Member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels))
                return BotServices.SendResponseAsync(ctx, "You are not allowed to see this channel!", ResponseType.Warning);

            // Create the base embed message
            var output = new DiscordEmbedBuilder()
                .WithTitle(channel.Name + " (" + channel.Id + ")")
                .WithDescription("Topic: " + (channel.IsCategory ? "N/A" : channel.Topic ?? string.Empty))
                .AddField("Type", channel.Type.ToString(), true)
                .AddField("Private", channel.IsPrivate ? "Yes" : "No", true)
                .AddField("NSFW", channel.IsNSFW ? "Yes" : "No", true)
                .WithThumbnail(ctx.Guild.IconUrl)
                .WithFooter("Created on " + channel.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture))
                .WithColor(Program.Settings.DefaultColor);

            // Add additional fields depending on the channel type
            switch (channel.Type)
            {
                case ChannelType.Voice:
                    output.AddField("Bitrate", channel.Bitrate.ToString(), true);
                    output.AddField("User limit", channel.UserLimit > 0 ? channel.UserLimit.ToString() : "No limit.", true);
                    break;

                case ChannelType.Category:
                    var channels = new StringBuilder();
                    foreach (var chn in channel.Children)
                        channels.Append($"[`{chn.Name}`]");
                    output.AddField("Channels", channels.Length > 0 ? channels.ToString() : "None", true);
                    break;
            }

            return ctx.CreateResponseAsync(output.Build());
        }

        [SlashCommand("purge", "Remove server user's channel messages.")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task Purge(InteractionContext ctx, [Option("member", "Server user whose messages will be purged.")] DiscordUser user, [Option("limit", "Number of messages to purge.")] double limit = 0)
        {
            var messages = await ctx.Channel.GetMessagesAsync(BotServices.LimitToRange(limit)).ConfigureAwait(false);
            await ctx.Channel.DeleteMessagesAsync(messages.Where(m => m.Author.Id == user.Id)).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"Purged **{limit}** messages by {user.Username}#{user.Discriminator} (ID:{user.Id})").ConfigureAwait(false);
        }

        [SlashCommand("rename", "Rename a channel. If a channel isn't specified, the current one will be used.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task SetChannelName(InteractionContext ctx, [Option("channel", "Channel to rename.")] DiscordChannel channel, [Option("name", "New channel name.")] string name)
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_NAME, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var oldName = channel.Name;
            await channel.ModifyAsync(m => m.Name = name.Trim().Replace(" ", "-")).ConfigureAwait(false);
            await ctx.CreateResponseAsync("Successfully renamed the channel " + Formatter.Bold(oldName) + " to " + Formatter.Bold(name)).ConfigureAwait(false);
        }

        [SlashCommand("text", "Create a new text channel.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task CreateText(InteractionContext ctx, [Option("name", "New text channel name.")] string name = "")
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_NAME, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            if (ctx.Guild.Channels.Any(chn => string.Equals(name, chn.Value.Name, StringComparison.OrdinalIgnoreCase)))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_EXISTS, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var channel = await ctx.Guild.CreateTextChannelAsync(name.Trim().Replace(" ", "-")).ConfigureAwait(false);
            await ctx.CreateResponseAsync("Successfully created the text channel " + Formatter.Bold(channel.Name)).ConfigureAwait(false);
        }

        [SlashCommand("topic", "Set current channel's topic.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task SetChannelTopic(InteractionContext ctx, [Option("topic", "New channel topic.")] string topic = "")
        {
            if (topic.Length > 1024)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_TOPIC, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await ctx.Channel.ModifyAsync(chn => chn.Topic = topic).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(topic))
                await ctx.CreateResponseAsync("Successfully changed the channel topic to " + Formatter.Bold(topic)).ConfigureAwait(false);
        }

        [SlashCommand("voice", "Create a new voice channel.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task CreateVoice(InteractionContext ctx, [Option("name", "New voice channel name.")] string name = "")
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_NAME, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            if (ctx.Guild.Channels.Any(chn => string.Equals(name, chn.Value.Name, StringComparison.OrdinalIgnoreCase)))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_EXISTS, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var channel = await ctx.Guild.CreateVoiceChannelAsync(name.Trim().Replace(" ", "-")).ConfigureAwait(false);
            await ctx.CreateResponseAsync("Successfully created the voice channel " + Formatter.Bold(channel.Name)).ConfigureAwait(false);
        }
    }
}