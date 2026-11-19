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
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await MiscService.GetCatImageAsync().ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.ERR_API_CONNECTION, ResponseType.Warning).ConfigureAwait(false);
        }

        [SlashCommand("coinflip", "Flips a coin.")]
        public async Task GetCoinFlip(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $":coin: {ctx.User.Username} flipped a coin and got {Formatter.Bold(Random.Shared.Next(2) == 0 ? "Heads" : "Tails")}.").ConfigureAwait(false);
        }

        [SlashCommand("diceroll", "Rolls a six-sided die.")]
        public async Task GetRollDice(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $":game_die: {ctx.User.Username} rolled a die and got {Formatter.Bold(Random.Shared.Next(1, 7).ToString())}.").ConfigureAwait(false);
        }

        [SlashCommand("dog", "Returns a random dog image.")]
        public async Task GetDogImage(InteractionContext ctx)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await MiscService.GetDogImageAsync().ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.ERR_API_CONNECTION, ResponseType.Warning).ConfigureAwait(false);
        }
    }
}
