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
    [SlashCommandGroup("channel", "Slash command group for channel commands.")]
    public class ChannelModule : ApplicationCommandModule
    {
        /// <summary>
        /// Creates a new channel category.
        /// </summary>
        [SlashCommand("new-category", "Creates a new channel category.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task CreateCategory(InteractionContext ctx, [Option("name", "New category name.")] string name)
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CATEGORY_NAME, ResponseType.Warning).ConfigureAwait(false);
                return;
            }
            await ctx.Guild.CreateChannelCategoryAsync(name.Trim()).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"Created channel category {Formatter.Bold(name)}.").ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new text channel.
        /// </summary>
        [SlashCommand("new-text", "Creates a new text channel.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task CreateText(InteractionContext ctx, [Option("name", "New text channel name.")] string name)
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_NAME, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            if (ctx.Guild.Channels.Any(x => string.Equals(x.Value.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_EXISTS, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var channel = await ctx.Guild.CreateTextChannelAsync(name.Trim().Replace(" ", "-")).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"Created text channel #{Formatter.Bold(channel.Name)}.").ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new voice channel.
        /// </summary>
        [SlashCommand("new-voice", "Creates a new voice channel.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task CreateVoice(InteractionContext ctx, [Option("name", "New voice channel name.")] string name)
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_NAME, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            if (ctx.Guild.Channels.Any(x => string.Equals(x.Value.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_EXISTS, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var channel = await ctx.Guild.CreateVoiceChannelAsync(name.Trim().Replace(" ", "-")).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"Created voice channel #{Formatter.Bold(channel.Name)}.").ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes server channel.
        /// </summary>
        [SlashCommand("delete", "Delete server channel.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task DeleteChannel(InteractionContext ctx, [Option("channel", "Channel to delete.")] DiscordChannel channel)
        {
            await channel.DeleteAsync().ConfigureAwait(false);
            await ctx.CreateResponseAsync($"Deleted channel #{Formatter.Bold(channel.Name)}.").ConfigureAwait(false);
        }

        /// <summary>
        /// Returns information about a server channel.
        /// </summary>
        [SlashCommand("info", "Returns information about a server channel.")]
        public Task GetChannelInfo(InteractionContext ctx, [Option("channel", "Channel to retrieve information from.")] DiscordChannel channel)
        {
            // Check that the user has the permission in the channel to view its information
            if (!ctx.Member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels))
                return BotServices.SendResponseAsync(ctx, "You are not allowed to see this channel!", ResponseType.Warning);

            var output = new DiscordEmbedBuilder()
                .WithTitle(channel.Name)
                .AddField("Type", channel.Type.ToString(), true)
                .AddField("Created On", channel.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture).Split(' ').First(), true)
                .WithThumbnail(ctx.Guild.IconUrl)
                .WithFooter($"ID: {channel.Id}")
                .WithColor(Program.Settings.DefaultColor);

            // Add topic if it exists and target is not a category.
            // BUG - DSharpPlus returns null even though a channel has a topic.
            if (!channel.IsCategory && channel.Topic is not null)
                output.WithDescription($"Topic: {channel.Topic}");

            // Add additional fields depending on the channel type
            switch (channel.Type)
            {
                case ChannelType.Voice:
                    output.AddField("Bitrate", channel.Bitrate is not null ? channel.Bitrate.ToString() : "Not set.");
                    output.AddField("User limit", channel.UserLimit > 0 ? channel.UserLimit.ToString() : "No limit.", true);
                    break;

                case ChannelType.Category:
                    var channels = new StringBuilder();
                    foreach (var x in channel.Children)
                        channels.Append(Formatter.BlockCode(x.Name));
                    output.AddField("Channels", channels.Length > 0 ? channels.ToString() : "None");
                    break;
            }

            return ctx.CreateResponseAsync(output.Build());
        }

        /// <summary>
        /// Removes messages from current channel.
        /// </summary>
        [SlashCommand("clean", "Removes messages from current channel.")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task DeleteChannelMessages(InteractionContext ctx, [Option("count", "Number of messages to remove from the current channel.")] double count)
        {
            var messages = await ctx.Channel.GetMessagesAsync(BotServices.LimitToRange(count)).ConfigureAwait(false);
            await ctx.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"Removed {Formatter.Bold(messages.Count.ToString())} message(s) removed from #{ctx.Channel.Name}.").ConfigureAwait(false);
        }

        /// <summary>
        /// Rename a server channel.
        /// </summary>
        [SlashCommand("rename", "Rename a server channel.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task SetChannelName(InteractionContext ctx, [Option("channel", "Channel to rename.")] DiscordChannel channel, [Option("name", "New channel name.")] string name)
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_NAME, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var oldName = channel.Name;
            await channel.ModifyAsync(x => x.Name = name.Trim().Replace(" ", "-")).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"Renamed channel #{Formatter.Bold(oldName)} to #{Formatter.Bold(name)}.").ConfigureAwait(false);
        }

        /// <summary>
        /// Changes the server channel topic.
        /// </summary>
        [SlashCommand("topic", "Changes the server channel topic.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task SetChannelTopic(InteractionContext ctx, [Option("topic", "New channel topic.")] string topic = "")
        {
            if (topic.Length > 1024)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_TOPIC, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await ctx.Channel.ModifyAsync(x => x.Topic = topic).ConfigureAwait(false);
            await ctx.CreateResponseAsync("Updated channel topic.").ConfigureAwait(false);
        }
    }
}