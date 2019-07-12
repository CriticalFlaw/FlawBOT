using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Net.Models;
using FlawBOT.Common;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System;
using System.Collections.Generic;
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
            if (string.IsNullOrWhiteSpace(name) || (name.Length > 100))
                await BotServices.SendEmbedAsync(ctx, "Category name cannot be blank or over 100 characters!", EmbedType.Warning);
            else
            {
                var category = await ctx.Guild.CreateChannelCategoryAsync(name.Trim());
                await BotServices.SendEmbedAsync(ctx, "Successfully created category " + Formatter.Bold(category.Name), EmbedType.Good);
            }
        }

        #endregion COMMAND_CATEGORY

        #region CHANNEL_CLEAN

        [Command("clean")]
        [Aliases("clear")]
        [Description("Remove channel messages")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task Clean(CommandContext ctx,
            [Description("Number of message to remove from the current channel")] int limit = 1)
        {
            if (limit <= 0 || limit > 100)
                await BotServices.SendEmbedAsync(ctx, "Invalid number of messages to delete, please enter a number 1-100!", EmbedType.Warning);
            else
            {
                var messages = await ctx.Channel.GetMessagesAsync(limit).ConfigureAwait(false);
                await ctx.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, Formatter.Bold(messages.Count.ToString()) + " message(s) removed from #" + ctx.Channel.Name, EmbedType.Good);
            }
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

            var prompt = await ctx.RespondAsync("You're about to delete the **" + channel + "**.\nRespond with **yes** if you want to proceed or wait 10 seconds to cancel the operation.");
            var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "yes", TimeSpan.FromSeconds(10));
            if (interactivity.Result == null) return;
            await BotServices.RemoveMessage(interactivity.Result);
            await BotServices.RemoveMessage(prompt);
            await BotServices.SendEmbedAsync(ctx, "Successfully deleted " + Formatter.Bold(channel.Name), EmbedType.Good);
            await channel.DeleteAsync();
        }

        #endregion COMMAND_DELETE

        #region COMMAND_INFO

        [Command("info")]
        [Aliases("i")]
        [Description("Print channel information. If a channel isn't specified, the current one will be used")]
        public Task GetChannel(CommandContext ctx,
            [Description("Channel to retrieve information from")] [RemainingText] DiscordChannel channel = null)
        {
            // Set the current channel for deletion if one isn't provided by the user
            channel = channel ?? ctx.Channel;

            if (!ctx.Member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels))
                return BotServices.SendEmbedAsync(ctx, "You are not allowed to see this channel!", EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(channel.Name + $" (ID: {channel.Id})")
                    .WithDescription("Channel topic: " + Formatter.Italic(string.IsNullOrWhiteSpace(channel.Topic) ? "" : channel.Topic))
                    .AddField("Type", channel.Type.ToString(), true)
                    .AddField("Private", channel.IsPrivate ? "YES" : "NO", true)
                    .AddField("NSFW", channel.IsNSFW ? "YES" : "NO", true)
                    .WithThumbnailUrl(ctx.Guild.IconUrl)
                    .WithFooter("Created on " + channel.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture))
                    .WithColor(SharedData.DefaultColor);
                if (channel.Type == ChannelType.Voice)
                {
                    output.AddField("Bitrate", channel.Bitrate.ToString(), true);
                    output.AddField("User limit", channel.UserLimit == 0 ? "No limit." : channel.UserLimit.ToString(), true);
                }
                else if (channel.Type == ChannelType.Category)
                {
                    var channels = new StringBuilder();
                    foreach (var chn in channel.Children)
                        channels.Append($"[`{chn.Name}`]");
                    if (channels.Length == 0) channels.Append("None");
                    output.AddField("Channels", channels.ToString(), true);
                }
                return ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_INFO

        #region CHANNEL_PURGE

        [Command("purge")]
        [Description("Remove server user's channel messages")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task Purge(CommandContext ctx,
            [Description("Server user whose messages will be purged")] DiscordMember member,
            [Description("Number of messages to purge")] [RemainingText] int limit = 0)
        {
            if (limit <= 0 || limit > 100)
                await BotServices.SendEmbedAsync(ctx, "Invalid number of messages to delete, please enter a number 1-100!", EmbedType.Warning);
            else
            {
                IReadOnlyList<DiscordMessage> msgs = await ctx.Channel.GetMessagesAsync(limit);
                await ctx.Channel.DeleteMessagesAsync(msgs.Where(m => m.Author.Id == member.Id));
                await BotServices.SendEmbedAsync(ctx, $"Purged **{limit}** messages by {member.Username}#{member.Discriminator} (ID:{member.Id})", EmbedType.Good);
            }
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
            if (string.IsNullOrWhiteSpace(name) || (name.Length > 100))
                await BotServices.SendEmbedAsync(ctx, "Channel name cannot be blank or over 100 characters!", EmbedType.Warning);
            else
            {
                string old_name = channel.Name;
                await channel.ModifyAsync(new Action<ChannelEditModel>(m => m.Name = name.Trim().Replace(" ", "-")));
                await BotServices.SendEmbedAsync(ctx, $"Successfully renamed the channel " + Formatter.Bold(old_name) + " to " + Formatter.Bold(name), EmbedType.Good);
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
            if (string.IsNullOrWhiteSpace(name) || (name.Length > 100))
                await BotServices.SendEmbedAsync(ctx, "Channel name cannot be blank or over 100 characters!", EmbedType.Warning);
            else if (ctx.Guild.Channels.Any(chn => string.Compare(name, chn.Value.Name, true) == 0))
                await BotServices.SendEmbedAsync(ctx, "Channel with the same name already exists!", EmbedType.Warning);
            else
            {
                var channel = await ctx.Guild.CreateTextChannelAsync(name.Trim().Replace(" ", "-"));
                await BotServices.SendEmbedAsync(ctx, "Successfully created the text channel " + Formatter.Bold(channel.Name), EmbedType.Good);
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
                await BotServices.SendEmbedAsync(ctx, "Channel topic must be less than 1024 characters long!", EmbedType.Warning);
            else
            {
                await ctx.Channel.ModifyAsync(chn => chn.Topic = topic);
                if (!string.IsNullOrWhiteSpace(topic))
                    await BotServices.SendEmbedAsync(ctx, "Successfully changed the channel topic to " + Formatter.Bold(topic), EmbedType.Good);
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
            if (string.IsNullOrWhiteSpace(name) || (name.Length > 100))
                await BotServices.SendEmbedAsync(ctx, "Channel name cannot be blank or over 100 characters!", EmbedType.Warning);
            else if (ctx.Guild.Channels.Any(chn => string.Compare(name, chn.Value.Name, true) == 0))
                await BotServices.SendEmbedAsync(ctx, "Channel with the same name already exists!", EmbedType.Warning);
            else
            {
                var channel = await ctx.Guild.CreateVoiceChannelAsync(name: name.Trim().Replace(" ", "-"));
                await BotServices.SendEmbedAsync(ctx, "Successfully created the voice channel " + Formatter.Bold(channel.Name), EmbedType.Good);
            }
        }

        #endregion COMMAND_VOICE
    }
}