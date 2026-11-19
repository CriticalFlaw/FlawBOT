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
    [SlashCommandGroup("channel", "Slash command group for channel commands.")]
    [SlashRequireGuild]
    public class ChannelModule : ApplicationCommandModule
    {
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
            await ctx.CreateResponseAsync($"Created channel category {Formatter.Bold(name.Trim())}.").ConfigureAwait(false);
        }

        [SlashCommand("new-text", "Creates a new text channel.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public Task CreateText(InteractionContext ctx, [Option("name", "New text channel name.")] string name)
        {
            return CreateChannel(ctx, name, ChannelType.Text);
        }

        [SlashCommand("new-voice", "Creates a new voice channel.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public Task CreateVoice(InteractionContext ctx, [Option("name", "New voice channel name.")] string name)
        {
            return CreateChannel(ctx, name, ChannelType.Voice);
        }

        private static async Task CreateChannel(InteractionContext ctx, string name, ChannelType type)
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_NAME, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            name = NormalizeChannelName(name, type);
            if (ctx.Guild.Channels.Values.Any(x => x.Type == type && string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_EXISTS, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var channel = type == ChannelType.Voice
                ? await ctx.Guild.CreateVoiceChannelAsync(name).ConfigureAwait(false)
                : await ctx.Guild.CreateTextChannelAsync(name).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"Created {(type == ChannelType.Voice ? "voice" : "text")} channel {channel.Mention}.").ConfigureAwait(false);
        }

        /// <summary>
        ///     Text channel names can't contain spaces, so Discord would otherwise replace them with dashes.
        /// </summary>
        public static string NormalizeChannelName(string name, ChannelType type)
        {
            name = name.Trim();
            return type == ChannelType.Voice ? name : name.Replace(' ', '-').ToLowerInvariant();
        }

        [SlashCommand("delete", "Deletes a server channel.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task DeleteChannel(InteractionContext ctx, [Option("channel", "Channel to delete.")] DiscordChannel channel)
        {
            var message = $"Deleted channel #{Formatter.Bold(channel.Name)}.";
            if (channel.Id == ctx.Channel.Id)
            {
                // Respond first; the interaction can't be answered once its channel is gone.
                await ctx.CreateResponseAsync(message).ConfigureAwait(false);
                await channel.DeleteAsync().ConfigureAwait(false);
                return;
            }

            await channel.DeleteAsync().ConfigureAwait(false);
            await ctx.CreateResponseAsync(message).ConfigureAwait(false);
        }

        [SlashCommand("info", "Returns information about a server channel.")]
        public Task GetChannelInfo(InteractionContext ctx, [Option("channel", "Channel to retrieve information from.")] DiscordChannel channel)
        {
            // Check that the user has the permission in the channel to view its information
            if (!ctx.Member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels))
                return BotServices.SendResponseAsync(ctx, "You are not allowed to see this channel!", ResponseType.Warning);

            var output = new DiscordEmbedBuilder()
                .WithTitle(channel.Name)
                .AddSafeField("Type", channel.Type.ToString(), true)
                .AddSafeField("Created On", channel.CreationTimestamp.UtcDateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), true)
                .WithFooter($"ID: {channel.Id}")
                .WithColor(Program.Settings.DefaultColor);
            if (!string.IsNullOrEmpty(ctx.Guild.IconUrl)) output.WithThumbnail(ctx.Guild.IconUrl);

            // Add topic if it exists and target is not a category.
            if (!channel.IsCategory && !string.IsNullOrWhiteSpace(channel.Topic))
                output.WithDescription($"Topic: {channel.Topic}");

            // Add additional fields depending on the channel type
            switch (channel.Type)
            {
                case ChannelType.Voice:
                    output.AddSafeField("Bitrate", channel.Bitrate is not null ? $"{channel.Bitrate / 1000} kbps" : "Not set.", true);
                    output.AddSafeField("User limit", channel.UserLimit > 0 ? channel.UserLimit.ToString() : "No limit.", true);
                    break;
                case ChannelType.Category:
                    output.AddSafeField("Channels", channel.Children.Select(x => Formatter.InlineCode(x.Name)).JoinWithinLimit(" ") is { Length: > 0 } list ? list : "None");
                    break;
            }

            return ctx.CreateResponseAsync(output.Build());
        }

        [SlashCommand("clean", "Removes messages from the current server channel.")]
        [SlashRequirePermissions(Permissions.ManageMessages)]
        public async Task DeleteChannelMessages(InteractionContext ctx, [Option("count", "Number of messages to remove from the current channel (1-100).")][Minimum(1)][Maximum(100)] long count)
        {
            // Respond privately so the response itself isn't one of the messages being cleaned.
            await ctx.DeferAsync(true).ConfigureAwait(false);

            // Discord only allows bulk deletion of messages younger than two weeks.
            var cutoff = DateTimeOffset.UtcNow.AddDays(-14).AddMinutes(5);
            var messages = (await ctx.Channel.GetMessagesAsync(BotServices.LimitToRange(count)).ConfigureAwait(false))
                .Where(x => x.CreationTimestamp > cutoff).ToList();

            if (messages.Count == 1)
                await messages[0].DeleteAsync().ConfigureAwait(false);
            else if (messages.Count > 1)
                await ctx.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);

            await BotServices.SendDeferredAsync(ctx, $"Removed {Formatter.Bold(messages.Count.ToString())} message(s) from #{ctx.Channel.Name}.").ConfigureAwait(false);
        }

        [SlashCommand("rename", "Changes the server channel name.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task SetChannelName(InteractionContext ctx, [Option("channel", "Channel to rename.")] DiscordChannel channel, [Option("name", "New channel name.")] string name)
        {
            if (!BotServices.CheckChannelName(name))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_CHANNEL_NAME, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var oldName = channel.Name;
            var newName = NormalizeChannelName(name, channel.Type);
            await channel.ModifyAsync(x => x.Name = newName).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"Renamed channel #{Formatter.Bold(oldName)} to #{Formatter.Bold(newName)}.").ConfigureAwait(false);
        }

        [SlashCommand("topic", "Changes the server channel topic.")]
        [SlashRequirePermissions(Permissions.ManageChannels)]
        public async Task SetChannelTopic(InteractionContext ctx, [Option("topic", "New channel topic. Leave empty to clear.")] string topic = "")
        {
            topic ??= string.Empty;
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
