using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class PollModule : BaseCommandModule
    {
        #region COMMAND_POLL

        [Command("poll")]
        [Description("Run a Yay or Nay poll in the current channel")]
        public async Task Poll(CommandContext ctx,
            [Description("Question to be polled")] [RemainingText] string question)
        {
            if (!BotServices.CheckUserInput(question))
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_POLL_QUESTION, EmbedType.Warning).ConfigureAwait(false);
            else
            {
                var interactivity = ctx.Client.GetInteractivity();
                var pollOptions = new List<DiscordEmoji>();
                pollOptions.Add(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
                pollOptions.Add(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));
                var duration = new TimeSpan(0, 0, 3, 0, 0);
                var output = new DiscordEmbedBuilder().WithDescription(ctx.User.Mention + "asked: " + question + "\nThis poll ends in 3 minutes.");
                var message = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
                foreach (var react in pollOptions)
                    await message.CreateReactionAsync(react).ConfigureAwait(false);
                var pollResult = await interactivity.CollectReactionsAsync(message, duration).ConfigureAwait(false);
                var results = pollResult.Where(x => pollOptions.Contains(x.Emoji)).Select(x => $"{x.Emoji} wins the poll with **{x.Total}** votes");
                await ctx.RespondAsync(string.Join("\n", results)).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_POLL
    }
}