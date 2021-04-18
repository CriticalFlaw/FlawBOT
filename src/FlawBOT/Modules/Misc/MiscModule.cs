using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using Newtonsoft.Json.Linq;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class MiscModule : BaseCommandModule
    {
        #region COMMAND_ASK

        [Command("ask")]
        [Aliases("8b", "8ball", "ball", "8")]
        [Description("Ask an 8-ball a question.")]
        public Task EightBall(CommandContext ctx,
            [Description("Question to ask the 8-ball.")] [RemainingText]
            string question = "")
        {
            if (string.IsNullOrWhiteSpace(question)) return Task.CompletedTask;
            var output = new DiscordEmbedBuilder()
                .WithDescription(":8ball: " + MiscService.GetRandomAnswer() + " (" + ctx.User.Mention + ")")
                .WithColor(DiscordColor.Black);
            return ctx.RespondAsync(output.Build());
        }

        #endregion COMMAND_ASK

        #region COMMAND_CAT

        [Command("cat")]
        [Aliases("meow", "catfact", "randomcat")]
        [Description("Retrieve a random cat fact and picture.")]
        public async Task GetCat(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var results = MiscService.GetCatFactAsync().Result;
            var output = new DiscordEmbedBuilder()
                .WithFooter($"Fact: {JObject.Parse(results)["fact"]}")
                .WithColor(DiscordColor.Orange);

            var image = MiscService.GetCatPhotoAsync().Result;
            if (!string.IsNullOrWhiteSpace(image))
                output.WithImageUrl(image);

            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_CAT

        #region COMMAND_COINFLIP

        [Command("coinflip")]
        [Aliases("coin", "flip")]
        [Description("Flip a coin.")]
        public Task CoinFlip(CommandContext ctx)
        {
            var random = new Random();
            var output = new DiscordEmbedBuilder()
                .WithDescription(ctx.User.Username + " flipped a coin and got " +
                                 Formatter.Bold(Convert.ToBoolean(random.Next(0, 2)) ? "Heads" : "Tails"))
                .WithColor(Program.Settings.DefaultColor);
            return ctx.RespondAsync(output.Build());
        }

        #endregion COMMAND_COINFLIP

        #region COMMAND_DICEROLL

        [Command("diceroll")]
        [Aliases("dice", "roll", "rolldice", "die")]
        [Description("Roll a six-sided die.")]
        public Task RollDice(CommandContext ctx)
        {
            var random = new Random();
            var output = new DiscordEmbedBuilder()
                .WithDescription(ctx.User.Username + " rolled a die and got " +
                                 Formatter.Bold(random.Next(1, 7).ToString()))
                .WithColor(Program.Settings.DefaultColor);
            return ctx.RespondAsync(output.Build());
        }

        #endregion COMMAND_DICEROLL

        #region COMMAND_DOG

        [Command("dog")]
        [Aliases("woof", "bark", "randomdog")]
        [Description("Retrieve a random dog photo")]
        public async Task GetDog(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var results = MiscService.GetDogPhotoAsync().Result;
            if (results.Status != "success")
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_API_CONNECTION, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            var output = new DiscordEmbedBuilder()
                .WithImageUrl(results.Message)
                .WithColor(DiscordColor.Brown);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_DOG

        #region COMMAND_HELLO

        [Command("hello")]
        [Aliases("hi", "howdy")]
        [Description("Say hello to another user to the server.")]
        public async Task Greet(CommandContext ctx,
            [Description("User to say hello to.")] [RemainingText]
            DiscordMember member)
        {
            if (member is null)
                await ctx.RespondAsync($"Hello, {ctx.User.Mention}").ConfigureAwait(false);
            else
                await ctx.RespondAsync($"Welcome {member.Mention} to {ctx.Guild.Name}. Enjoy your stay!")
                    .ConfigureAwait(false);
        }

        #endregion COMMAND_HELLO

        #region COMMAND_TTS

        [Command("tts")]
        [Aliases("echo", "repeat", "say", "talk")]
        [Description("Make FlawBOT repeat a message as text-to-speech.")]
        public async Task Say(CommandContext ctx,
            [Description("Message for FlawBOT to repeat.")] [RemainingText]
            string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                await ctx.RespondAsync(":thinking:").ConfigureAwait(false);
            else
                await ctx.RespondAsync(Formatter.BlockCode(Formatter.Strip(message))).ConfigureAwait(false);
        }

        #endregion COMMAND_TTS
    }
}