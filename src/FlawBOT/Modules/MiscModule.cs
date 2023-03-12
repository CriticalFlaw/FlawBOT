using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class MiscModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns an 8-ball response to a question.
        /// </summary>
        [SlashCommand("ask", "Returns an 8-ball response to a question.")]
        public async Task GetEightBallResponse(InteractionContext ctx, [Option("question", "Question to ask the 8-ball.")] string question = "")
        {
            var output = new DiscordEmbedBuilder()
                .WithDescription($":8ball: {MiscService.GetRandomAnswer()} ({ctx.User.Mention})")
                .WithColor(DiscordColor.Black);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a random cat fact and image.
        /// </summary>
        [SlashCommand("cat", "Returns a random cat fact and image.")]
        public async Task GetCatImage(InteractionContext ctx)
        {
            var output = MiscService.GetCatImageAsync().Result;
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_API_CONNECTION, ResponseType.Warning).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns result of a coin flip.
        /// </summary>
        [SlashCommand("coinflip", "Flip a coin.")]
        public async Task GetCoinFlip(InteractionContext ctx)
        {
            var output = new DiscordEmbedBuilder()
                .WithDescription(ctx.User.Username + " flipped a coin and got " + Formatter.Bold(Convert.ToBoolean(new Random().Next(0, 2)) ? "Heads" : "Tails"))
                .WithColor(Program.Settings.DefaultColor);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns result of a dice roll.
        /// </summary>
        [SlashCommand("diceroll", "Roll a six-sided die.")]
        public async Task GetRollDice(InteractionContext ctx)
        {
            var output = new DiscordEmbedBuilder()
                .WithDescription(ctx.User.Username + " rolled a die and got " + Formatter.Bold(new Random().Next(1, 7).ToString()))
                .WithColor(Program.Settings.DefaultColor);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a random dog image.
        /// </summary>
        [SlashCommand("dog", "Retrieve a random dog photo.")]
        public async Task GetDogImage(InteractionContext ctx)
        {
            var output = MiscService.GetDogImageAsync().Result;
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_API_CONNECTION, ResponseType.Warning).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a text message as text-to-speech.
        /// </summary>
        [SlashCommand("tts", "Returns a text message as text-to-speech.")]
        public async Task Say(InteractionContext ctx, [Option("message", "Message for FlawBOT to repeat.")] string message = "")
        {
            if (string.IsNullOrWhiteSpace(message))
                await ctx.CreateResponseAsync(":thinking:").ConfigureAwait(false);
            else
                await ctx.CreateResponseAsync(Formatter.BlockCode(Formatter.Strip(message))).ConfigureAwait(false);
        }

        //[SlashCommand("vote", "Run a Yay or Nay poll in the current channel.")]
        //public async Task Poll(CommandContext ctx, [Option("question", "Question to be asked in the poll.")] string question)
        //{
        //    if (string.IsNullOrWhiteSpace(question))
        //    {
        //        await BotServices.SendResponseAsync(ctx, Resources.ERR_POLL_QUESTION, ResponseType.Warning).ConfigureAwait(false);
        //        return;
        //    }

        //    // TODO - Not yet implemented
        //    // Build the poll question, duration and options.
        //    await ctx.TriggerTypingAsync();
        //    question = ctx.User.Mention + " asked: " + question;
        //    var interactivity = ctx.Client.GetInteractivity();
        //    var pollOptions = new List<DiscordEmoji>
        //    {
        //        DiscordEmoji.FromName(ctx.Client, ":thumbsup:"),
        //        DiscordEmoji.FromName(ctx.Client, ":thumbsdown:")
        //    };
        //    var duration = new TimeSpan(0, 3, 10);
        //    var message = await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription(question + $"\nThis poll ends in {duration.Minutes} minutes.").Build()).ConfigureAwait(false);
        //    var results = await interactivity.DoPollAsync(message, pollOptions, PollBehaviour.DeleteEmojis, duration).ConfigureAwait(false);

        //    // Removed the initial poll and return the calculated results
        //    await BotServices.RemoveMessage(message).ConfigureAwait(false);
        //    var output = new DiscordEmbedBuilder()
        //        .WithDescription(question)
        //        .WithFooter("The voting has ended.");
        //    foreach (var vote in results)
        //        output.AddField(vote.Emoji.Name, vote.Voted.Count.ToString(), true);
        //    await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        //}
    }
}