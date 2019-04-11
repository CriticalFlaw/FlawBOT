﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Net.Models;
using FlawBOT.Models;
using FlawBOT.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Server
{
    [Group("channel")]
    [Description("Channel admin.")]
    [Aliases("chn", "ch", "c")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class ChannelModule : BaseCommandModule
    {
        #region CHANNEL_CREATETEXT

        [Command("createtext")]
        [Description("Create a new text channel.")]
        [Aliases("ctc")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task CreateTextChannel(CommandContext ctx, [RemainingText] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                await BotServices.SendEmbedAsync(ctx, ":warning: Channel name cannot be blank!", EmbedType.Warning);
            else if (name.Length > 100)
                await BotServices.SendEmbedAsync(ctx, ":warning: Channel name must be less than 100 characters long!", EmbedType.Warning);
            else if (ctx.Guild.Channels.Any(chn => string.Compare(name, chn.Name, true) == 0))
                await BotServices.SendEmbedAsync(ctx, ":warning: Channel with the same name already exists!", EmbedType.Warning);
            else
            {
                var channel = await ctx.Guild.CreateTextChannelAsync(name.Trim().Replace(" ", "-"));
                await BotServices.SendEmbedAsync(ctx, "Successfully created text channel #" + Formatter.Bold(channel.Name), EmbedType.Good);
            }
        }

        #endregion CHANNEL_CREATETEXT

        #region CHANNEL_CREATEVOICE

        [Command("createvoice")]
        [Description("Create a new voice channel.")]
        [Aliases("cvc")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task CreateVoiceChannel(CommandContext ctx, string name, int? userlimit = null, int? bitrate = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                await BotServices.SendEmbedAsync(ctx, ":warning: Channel name cannot be blank!", EmbedType.Warning);
            else if (name.Length > 100)
                await BotServices.SendEmbedAsync(ctx, ":warning: Channel name must be less than 100 characters long!", EmbedType.Warning);
            else if (ctx.Guild.Channels.Any(chn => string.Compare(name, chn.Name, true) == 0))
                await BotServices.SendEmbedAsync(ctx, ":warning: Channel with the same name already exists!", EmbedType.Warning);
            else
            {
                var channel = await ctx.Guild.CreateVoiceChannelAsync(name: name, bitrate: bitrate, user_limit: userlimit);
                await BotServices.SendEmbedAsync(ctx, "Successfully created voice channel #" + Formatter.Bold(channel.Name), EmbedType.Good);
            }
        }

        #endregion CHANNEL_CREATEVOICE

        #region CHANNEL_CREATECATEGORY

        [Command("createcategory")]
        [Description("Create a new channel category.")]
        [Aliases("newcategory")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task CreateChannelCategory(CommandContext ctx, [RemainingText] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                await BotServices.SendEmbedAsync(ctx, ":warning: Category name cannot be blank!", EmbedType.Warning);
            else if (name.Length > 100)
                await BotServices.SendEmbedAsync(ctx, ":warning: Category name must be less than 100 characters long!", EmbedType.Warning);
            else
            {
                var category = await ctx.Guild.CreateChannelCategoryAsync(name.Trim());
                await BotServices.SendEmbedAsync(ctx, "Successfully created category " + Formatter.Bold(category.Name), EmbedType.Good);
            }
        }

        #endregion CHANNEL_CREATECATEGORY

        #region CHANNEL_DELETE

        [Command("delete")]
        [Description("Delete given channel. If a channel isn't provided, the current one will be deleted. A reason for the deletion can be specified.")]
        [Aliases("dtc")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task RemoveTextChannel(CommandContext ctx, DiscordChannel channel = null, [RemainingText] string reason = null)
        {
            // Set the current channel for deletion if one isn't provided by the user
            channel = channel ?? ctx.Channel;

            var prompt = await ctx.RespondAsync("You're about to delete the **current** channel. Respond with **yes** if you want to proceed or wait 10 seconds to cancel the operation.");

            var interactivity = await ctx.Client.GetInteractivity()
                .WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "yes", TimeSpan.FromSeconds(10));
            if (interactivity == null)
                await ctx.RespondAsync("Timed Out! Your report has **NOT** been submitted.");
            else
                await channel.DeleteAsync(reason);
        }

        #endregion CHANNEL_DELETE

        #region COMMAND_RENAME

        [Command("rename")]
        [Description("Rename a channel. If a channel isn't provided, the current one will be renamed.")]
        [Aliases("rn")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task SetChannelName(CommandContext ctx, DiscordChannel channel, [RemainingText] string name)
        {
            // Set the current channel for deletion if one isn't provided by the user
            channel = channel ?? ctx.Channel;

            if (string.IsNullOrWhiteSpace(name))
                await BotServices.SendEmbedAsync(ctx, ":warning: Channel name cannot be blank!", EmbedType.Warning);
            else if (name.Length < 2 || name.Length > 100)
                await BotServices.SendEmbedAsync(ctx, "Channel name must be between 2 and 100 characters.", EmbedType.Warning);

            string old_name = channel.Name;
            await channel.ModifyAsync(new Action<ChannelEditModel>(m => m.Name = name.Trim().Replace(" ", "-")));
            await BotServices.SendEmbedAsync(ctx, $"Successfully renamed channel {Formatter.Bold(old_name)} to {Formatter.Bold(name)}", EmbedType.Good);

            //await ctx.Channel.ModifyAsync(chn => chn.Name = name.Trim().Replace(" ", "-"));
            //await ctx.RespondAsync($"Channel name has been changed to **{name.Trim().Replace(" ", "-")}**");
        }

        #endregion COMMAND_RENAME

        #region CHANNEL_SETTOPIC

        [Command("settopic")]
        [Description("Set channel topic. If a channel isn't provided, the current one will be used.")]
        [Aliases("st")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task SetChannelTopic(CommandContext ctx, DiscordChannel channel, [RemainingText] string topic)
        {
            // Set the current channel for deletion if one isn't provided by the user
            channel = channel ?? ctx.Channel;

            if (string.IsNullOrWhiteSpace(topic))
                await BotServices.SendEmbedAsync(ctx, ":warning: Channel topic cannot be blank!", EmbedType.Warning);
            else if (topic.Length > 1024)
                await BotServices.SendEmbedAsync(ctx, ":warning: Channel topic must be less than 1024 characters long!", EmbedType.Warning);
            else
            {
                await ctx.Channel.ModifyAsync(chn => chn.Topic = topic);
                await BotServices.SendEmbedAsync(ctx, "Successfully updated topic for #" + Formatter.Bold(channel.Name), EmbedType.Good);
            }
        }

        #endregion CHANNEL_SETTOPIC

        #region CHANNEL_INFO

        [Command("info")]
        [Description("Print channel information. If a channel isn't provided, the current channel will be used.")]
        [Aliases("i")]
        public Task GetChannel(CommandContext ctx, DiscordChannel channel = null)
        {
            // Set the current channel for deletion if one isn't provided by the user
            channel = channel ?? ctx.Channel;

            if (!ctx.Member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels))
                return BotServices.SendEmbedAsync(ctx, ":warning: You are not allowed to see this channel!", EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(channel.Name)
                    .WithDescription($"Channel topic: {Formatter.Italic(string.IsNullOrWhiteSpace(channel.Topic) ? "None" : channel.Topic)}")
                    .WithColor(DiscordColor.Aquamarine)
                    .AddField("ID", channel.Id.ToString(), true)
                    .AddField("Type", channel.Type.ToString(), true)
                    .AddField("Private", channel.IsPrivate ? "YES" : "NO", true)
                    .AddField("NSFW", channel.IsNSFW ? "YES" : "NO", true)
                    .WithThumbnailUrl(ctx.Guild.IconUrl)
                    .WithFooter($"Created on {channel.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}")
                    .WithUrl($"https://discord.gg/{ctx.Channel.CreateInviteAsync().Result.Code}");
                if (channel.Type == ChannelType.Voice)
                {
                    output.AddField("Bitrate", channel.Bitrate.ToString(), true);
                    output.AddField("User limit", channel.UserLimit == 0 ? "No limit." : channel.UserLimit.ToString(), true);
                }
                return ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion CHANNEL_INFO
    }
}