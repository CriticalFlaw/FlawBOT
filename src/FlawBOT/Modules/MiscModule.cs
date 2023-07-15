using DSharpPlus;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class MiscModule : ApplicationCommandModule
    {
        [SlashCommand("ask", "Asks the magic 8-ball a question.")]
        public async Task GetRandomAnswer(InteractionContext ctx, [Option("question", "Question to ask the 8-ball.")] string question)
        {
            await BotServices.SendResponseAsync(ctx, $":8ball: {MiscService.GetRandomAnswer()}.").ConfigureAwait(false);
        }

        [SlashCommand("cat", "Returns a random cat fact and image.")]
        public async Task GetCatImage(InteractionContext ctx)
        {
            var output = await MiscService.GetCatImageAsync().ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_API_CONNECTION, ResponseType.Warning).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        [SlashCommand("coinflip", "Flips a coin.")]
        public async Task GetCoinFlip(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $":coin: {ctx.User.Username} flipped a coin and got {Formatter.Bold(Convert.ToBoolean(new Random().Next(0, 2)) ? "Heads" : "Tails")}.").ConfigureAwait(false);
        }

        [SlashCommand("diceroll", "Rolls a six-sided die.")]
        public async Task GetRollDice(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $":game_die: {ctx.User.Username} rolled a die and got {Formatter.Bold(new Random().Next(1, 7).ToString())}.").ConfigureAwait(false);
        }

        [SlashCommand("dog", "Returns a random dog image.")]
        public async Task GetDogImage(InteractionContext ctx)
        {
            var output = await MiscService.GetDogImageAsync().ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_API_CONNECTION, ResponseType.Warning).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        /*
        [SlashCommand("vote", "Run a Yay or Nay poll in the current channel.")]
        public async Task Poll(CommandContext ctx, [Option("question", "Question to be asked in the poll.")] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_POLL_QUESTION, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            // TODO - Not yet implemented
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
            var message = await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription(question + $"\nThis poll ends in {duration.Minutes} minutes.").Build()).ConfigureAwait(false);
            var results = await interactivity.DoPollAsync(message, pollOptions, PollBehaviour.DeleteEmojis, duration).ConfigureAwait(false);

            // Removed the initial poll and return the calculated results
            await BotServices.RemoveMessage(message).ConfigureAwait(false);
            var output = new DiscordEmbedBuilder()
                .WithDescription(question)
                .WithFooter("The voting has ended.");
            foreach (var vote in results)
                output.AddField(vote.Emoji.Name, vote.Voted.Count.ToString(), true);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }
        */
    }
}