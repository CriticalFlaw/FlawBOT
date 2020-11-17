using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class PollModule : BaseCommandModule
    {
        #region COMMAND_POLL

        [Command("poll"), Aliases("vote")]
        [Description("Run a Yay or Nay poll in the current channel")]
        public async Task Poll(CommandContext ctx,
            [Description("Question to be polled"), RemainingText]
            string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_POLL_QUESTION, EmbedType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            // Build the poll question, duration and options.
            question = ctx.User.Mention + " asked: " + question;
            var interactivity = ctx.Client.GetInteractivity();
            var pollOptions = new List<DiscordEmoji>
            {
                DiscordEmoji.FromName(ctx.Client, ":thumbsup:"),
                DiscordEmoji.FromName(ctx.Client, ":thumbsdown:")
            };
            var duration = new TimeSpan(0, 3, 10);
            var message = await ctx
                .RespondAsync(embed: new DiscordEmbedBuilder()
                    .WithDescription(question + $"\nThis poll ends in {duration.Minutes} minutes.").Build())
                .ConfigureAwait(false);
            var results = await interactivity.DoPollAsync(message, pollOptions, PollBehaviour.DeleteEmojis, duration);

            // Removed the initial poll and return the calculated results
            await BotServices.RemoveMessage(message).ConfigureAwait(false);
            var output = new DiscordEmbedBuilder()
                .WithDescription(question)
                .WithFooter("The voting has ended.");
            foreach (var vote in results)
                output.AddField(vote.Emoji.Name, vote.Voted.Count.ToString(), true);
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_POLL
    }
}