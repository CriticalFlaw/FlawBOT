using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class PollModule : BaseCommandModule
    {
        #region COMMAND_POLL

        [Command("poll")]
        [Aliases("vote")]
        [Description("Run a Yay or Nay poll in the current channel")]
        public async Task Poll(CommandContext ctx,
            [Description("Question to be polled")] [RemainingText]
            string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_POLL_QUESTION, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            // Build the poll question, duration and options.
            await ctx.TriggerTypingAsync();
            question = ctx.User.Mention + " asked: " + question;
            var interactivity = ctx.Client.GetInteractivity();
            var pollOptions = new List<DiscordEmoji>
            {
                DiscordEmoji.FromName(ctx.Client, ":thumbsup:"),
                DiscordEmoji.FromName(ctx.Client, ":thumbsdown:")
            };
            var duration = new TimeSpan(0, 3, 10);
            var message = await ctx
                .RespondAsync(new DiscordEmbedBuilder()
                    .WithDescription(question + $"\nThis poll ends in {duration.Minutes} minutes.").Build())
                .ConfigureAwait(false);
            var results = await interactivity.DoPollAsync(message, pollOptions, PollBehaviour.DeleteEmojis, duration)
                .ConfigureAwait(false);

            // Removed the initial poll and return the calculated results
            await BotServices.RemoveMessage(message).ConfigureAwait(false);
            var output = new DiscordEmbedBuilder()
                .WithDescription(question)
                .WithFooter("The voting has ended.");
            foreach (var vote in results)
                output.AddField(vote.Emoji.Name, vote.Voted.Count.ToString(), true);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_POLL
    }
}