using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using FlawBOT.Services.Lookup;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Misc
{
    public class MiscModule : ApplicationCommandModule
    {
        #region COMMAND_ASK

        [SlashCommand("ask", "Ask an 8-ball a question.")]
        public Task EightBall(InteractionContext ctx, [Option("query", "Question to ask the 8-ball.")] string question = "")
        {
            if (string.IsNullOrWhiteSpace(question)) return Task.CompletedTask;
            var output = new DiscordEmbedBuilder()
                .WithDescription(":8ball: " + MiscService.GetRandomAnswer() + " (" + ctx.User.Mention + ")")
                .WithColor(DiscordColor.Black);
            return ctx.CreateResponseAsync(output.Build());
        }

        #endregion COMMAND_ASK

        #region COMMAND_CAT

        [SlashCommand("cat", "Retrieve a random cat fact and picture.")]
        public async Task GetCat(InteractionContext ctx)
        {
            var results = MiscService.GetCatFactAsync().Result;
            var output = new DiscordEmbedBuilder()
                .WithFooter($"Fact: {JObject.Parse(results)["fact"]}")
                .WithColor(DiscordColor.Orange);

            var image = MiscService.GetCatPhotoAsync().Result;
            if (!string.IsNullOrWhiteSpace(image))
                output.WithImageUrl(image);

            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_CAT

        #region COMMAND_COINFLIP

        [SlashCommand("coinflip", "Flip a coin.")]
        public Task CoinFlip(InteractionContext ctx)
        {
            var random = new Random();
            var output = new DiscordEmbedBuilder()
                .WithDescription(ctx.User.Username + " flipped a coin and got " + Formatter.Bold(Convert.ToBoolean(random.Next(0, 2)) ? "Heads" : "Tails"))
                .WithColor(Program.Settings.DefaultColor);
            return ctx.CreateResponseAsync(output.Build());
        }

        #endregion COMMAND_COINFLIP

        #region COMMAND_DICEROLL

        [SlashCommand("diceroll", "Roll a six-sided die.")]
        public Task RollDice(InteractionContext ctx)
        {
            var random = new Random();
            var output = new DiscordEmbedBuilder()
                .WithDescription(ctx.User.Username + " rolled a die and got " + Formatter.Bold(random.Next(1, 7).ToString()))
                .WithColor(Program.Settings.DefaultColor);
            return ctx.CreateResponseAsync(output.Build());
        }

        #endregion COMMAND_DICEROLL

        #region COMMAND_DOG

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

        #endregion COMMAND_DOG

        #region COMMAND_HELLO

        [SlashCommand("hello", "Say hello to another user to the server.")]
        public async Task Greet(InteractionContext ctx, [Option("member", "User to say hello to.")] DiscordMember member)
        {
            if (member is null)
                await ctx.CreateResponseAsync($"Hello, {ctx.User.Mention}").ConfigureAwait(false);
            else
                await ctx.CreateResponseAsync($"Welcome {member.Mention} to {ctx.Guild.Name}. Enjoy your stay!").ConfigureAwait(false);
        }

        #endregion COMMAND_HELLO

        #region COMMAND_TTS

        [SlashCommand("tts", "Make FlawBOT repeat a message as text-to-speech.")]
        public async Task Say(InteractionContext ctx, [Option("message", "Message for FlawBOT to repeat.")] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                await ctx.CreateResponseAsync(":thinking:").ConfigureAwait(false);
            else
                await ctx.CreateResponseAsync(Formatter.BlockCode(Formatter.Strip(message))).ConfigureAwait(false);
        }

        #endregion COMMAND_TTS
    }
}