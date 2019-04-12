using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Server
{
    [Group("message")]
    [Aliases("msg", "msgs", "messages")]
    [Description("Commands for cleaning channel messages")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class MessagesModule : BaseCommandModule
    {
        #region CHANNEL_CLEAN

        [Command("clean")]
        [Aliases("clear")]
        [Description("Remove channel messages")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task Clean(CommandContext ctx,
            [Description("Number of message to remove from the current channel")] int limit = 0)
        {
            if (limit <= 0 || limit > 100)
                await BotServices.SendEmbedAsync(ctx, "Invalid number of messages to delete, must be in range of 1-100!", EmbedType.Warning);
            else
            {
                var messages = await ctx.Channel.GetMessagesAsync(limit).ConfigureAwait(false);
                await ctx.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, $"**{messages.Count}** message(s) have been removed from #{ctx.Channel.Name}", EmbedType.Good);
            }
        }

        #endregion CHANNEL_CLEAN

        #region CHANNEL_PURGE

        [Command("purge")]
        [Description("Remove server user's channel messages")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task Purge(CommandContext ctx,
            [Description("Server user whose messages will be purged")] DiscordMember member,
            [Description("Number of messages to purge")] [RemainingText] int limit = 10)
        {
            if (limit <= 0 || limit > 100)
                await BotServices.SendEmbedAsync(ctx, "Invalid number of messages to delete, must be in range of 1-100!", EmbedType.Warning);
            IReadOnlyList<DiscordMessage> msgs = await ctx.Channel.GetMessagesAsync(limit);
            await ctx.Channel.DeleteMessagesAsync(msgs.Where(m => m.Author.Id == member.Id));
            await BotServices.SendEmbedAsync(ctx, $"Purged **{limit}** messages by {member.Username}#{member.Discriminator} (ID:{member.Id})", EmbedType.Good);
        }

        #endregion CHANNEL_PURGE
    }
}