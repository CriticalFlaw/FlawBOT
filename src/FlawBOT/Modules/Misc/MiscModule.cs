using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Misc
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class MiscModule : BaseCommandModule
    {
        #region COMMAND_8BALL

        [Command("ask")]
        [Aliases("8b", "8ball")]
        [Description("Ask an 8-ball a question")]
        public Task EightBall(CommandContext ctx,
            [Description("Question to ask the 8-Ball")] [RemainingText] string question = "")
        {
            if (string.IsNullOrWhiteSpace(question))
                return BotServices.SendEmbedAsync(ctx, ":8ball: The almighty 8 ball requests a question", EmbedType.Warning);
            var output = new DiscordEmbedBuilder()
                .WithDescription(":8ball: " + ctx.User.Mention + " " + EightBallService.GetAnswer())
                .WithColor(DiscordColor.Black);
            return ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_8BALL

        #region COMMAND_CATFACT

        [Command("cat")]
        [Aliases("catfact")]
        [Description("Retrieve a random cat fact")]
        public async Task CatFact(CommandContext ctx)
        {
            var http = new HttpClient();
            var response = await http.GetStringAsync("https://catfact.ninja/fact");
            var output = new DiscordEmbedBuilder()
                .WithTitle($":cat: {JObject.Parse(response)["fact"]}")
                .WithColor(DiscordColor.Orange);
            await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_CATFACT

        #region COMMAND_CATPIC

        [Command("randomcat")]
        [Aliases("meow")]
        [Description("Retrieve a random cat photo")]
        public async Task CatPic(CommandContext ctx)
        {
            var http = new HttpClient();
            var results = await http.GetStringAsync("http://aws.random.cat/meow").ConfigureAwait(false);
            string url = JObject.Parse(results)["file"].ToString();

            if (string.IsNullOrWhiteSpace(url))
                await BotServices.SendEmbedAsync(ctx, "Connection to random.cat failed!", EmbedType.Warning);

            var output = new DiscordEmbedBuilder()
                .WithTitle(":cat: Meow!")
                .WithImageUrl(url)
                .WithColor(DiscordColor.Orange);
            await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_CATPIC

        #region COMMAND_COINFLIP

        [Command("coinflip")]
        [Aliases("coin", "flip")]
        [Description("Flip a coin")]
        public Task CoinFlip(CommandContext ctx)
        {
            var random = new Random();
            var output = new DiscordEmbedBuilder()
                .WithDescription(ctx.User.Mention + " flipped " + Formatter.Bold(Convert.ToBoolean(random.Next(0, 2)) ? "Heads" : "Tails"))
                .WithColor(DiscordColor.Black);
            return ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_COINFLIP

        #region COMMAND_COLOR

        [Command("color")]
        [Aliases("clr")]
        [Description("Retrieve color values for a given HEX code")]
        public async Task GetColor(CommandContext ctx,
            [Description("HEX color code to process")] DiscordColor color)
        {
            var regex = new Regex(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", RegexOptions.Compiled).Match(color.ToString());
            if (regex.Success)
            {
                var output = new DiscordEmbedBuilder()
                    .AddField("HEX:", $"{color.Value:X}", true)
                    .AddField("RGB:", $"{color.R} {color.G} {color.B}", true)
                    .AddField("Decimal:", color.Value.ToString(), true)
                    .WithColor(color);
                await ctx.RespondAsync(embed: output.Build());
            }
            else
                await BotServices.SendEmbedAsync(ctx, "Invalid color code. Please enter a HEX color code like #E7B53B", EmbedType.Warning);
        }

        #endregion COMMAND_COLOR

        #region COMMAND_DICEROLL

        [Command("diceroll")]
        [Aliases("dice", "roll", "rolldice")]
        [Description("Roll a six-sided die")]
        public Task RollDice(CommandContext ctx)
        {
            var random = new Random();
            var output = new DiscordEmbedBuilder()
                .WithDescription(ctx.User.Mention + " rolled a " + Formatter.Bold(random.Next(1, 7).ToString()))
                .WithColor(DiscordColor.Black);
            return ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_DICEROLL

        #region COMMAND_DOGPIC

        [Command("randomdog")]
        [Aliases("woof")]
        [Description("Retrieve a random dog photo")]
        public async Task DogPic(CommandContext ctx)
        {
            var results = DogService.GetDogPhotoAsync().Result;
            if (results.status != "success")
                await BotServices.SendEmbedAsync(ctx, "Unable to retrieve a pupper photo :(", EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(":dog: Woof!")
                    .WithImageUrl(results.message)
                    .WithColor(DiscordColor.Brown);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_DOGPIC

        #region COMMAND_HELLO

        [Command("hello")]
        [Aliases("hi", "howdy")]
        [Description("Welcome another user to the server")]
        public async Task Greet(CommandContext ctx,
            [Description("User to say hello to")] [RemainingText] DiscordMember member)
        {
            if (member == null)
                await ctx.RespondAsync($":wave: Hello, " + ctx.User.Mention);
            else
                await ctx.RespondAsync($":wave: Welcome " + member.Mention + " to " + ctx.Guild.Name + ". Enjoy your stay!");
        }

        #endregion COMMAND_HELLO

        #region COMMAND_TTS

        [Command("tts")]
        [Description("Sends a text-to-speech message")]
        [RequirePermissions(Permissions.SendTtsMessages)]
        public Task SayTTS(CommandContext ctx,
            [Description("Text to convert to speech")] [RemainingText] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return ctx.RespondAsync("I need something to say...");
            return ctx.RespondAsync(Formatter.BlockCode(Formatter.Strip(text)), isTTS: true);
        }

        #endregion COMMAND_TTS
    }
}