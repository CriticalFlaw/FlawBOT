using DSharpPlus;
using DSharpPlus.Entities;
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
        /// <summary>
        /// Returns an 8-ball response to a question.
        /// </summary>
        [SlashCommand("ask", "Ask an 8-ball a question.")]
        public Task EightBall(InteractionContext ctx, [Option("question", "Question to ask the 8-ball.")] string question)
        {
            if (string.IsNullOrWhiteSpace(question)) return Task.CompletedTask;
            var output = new DiscordEmbedBuilder()
                .WithDescription($":8ball: {MiscService.GetRandomAnswer()} ({ctx.User.Mention})")
                .WithColor(DiscordColor.Black);
            return ctx.CreateResponseAsync(output.Build());
        }

        /// <summary>
        /// Returns a random cat photo and fact.
        /// </summary>
        [SlashCommand("cat", "Retrieve a random cat fact and picture.")]
        public async Task GetCat(InteractionContext ctx)
        {
            var output = new DiscordEmbedBuilder()
                .WithFooter($"Fact: {MiscService.GetCatFactAsync().Result}")
                .WithColor(DiscordColor.Orange);

            var image = MiscService.GetCatPhotoAsync().Result;
            if (!string.IsNullOrWhiteSpace(image))
                output.WithImageUrl(image);

            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns result of a coin flip.
        /// </summary>
        [SlashCommand("coinflip", "Flip a coin.")]
        public Task CoinFlip(InteractionContext ctx)
        {
            var random = new Random();
            var output = new DiscordEmbedBuilder()
                .WithDescription(ctx.User.Username + " flipped a coin and got " + Formatter.Bold(Convert.ToBoolean(random.Next(0, 2)) ? "Heads" : "Tails"))
                .WithColor(Program.Settings.DefaultColor);
            return ctx.CreateResponseAsync(output.Build());
        }

        /// <summary>
        /// Returns result of a dice roll.
        /// </summary>
        [SlashCommand("diceroll", "Roll a six-sided die.")]
        public Task RollDice(InteractionContext ctx)
        {
            var random = new Random();
            var output = new DiscordEmbedBuilder()
                .WithDescription(ctx.User.Username + " rolled a die and got " + Formatter.Bold(random.Next(1, 7).ToString()))
                .WithColor(Program.Settings.DefaultColor);
            return ctx.CreateResponseAsync(output.Build());
        }

        /// <summary>
        /// Returns a random dog photo.
        /// </summary>
        [SlashCommand("dog", "Retrieve a random dog photo.")]
        public async Task GetDog(InteractionContext ctx)
        {
            var results = MiscService.GetDogPhotoAsync().Result;
            if (results.Status != "success")
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_API_CONNECTION, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var output = new DiscordEmbedBuilder()
                .WithImageUrl(results.Message)
                .WithColor(DiscordColor.Brown);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a text message as TTS.
        /// </summary>
        [SlashCommand("tts", "Make FlawBOT repeat a message as text-to-speech.")]
        public async Task Say(InteractionContext ctx, [Option("message", "Message for FlawBOT to repeat.")] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                await ctx.CreateResponseAsync(":thinking:").ConfigureAwait(false);
            else
                await ctx.CreateResponseAsync(Formatter.BlockCode(Formatter.Strip(message))).ConfigureAwait(false);
        }
    }
}