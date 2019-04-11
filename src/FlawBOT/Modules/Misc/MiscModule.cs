using BitlyAPI;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Misc
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class MiscModule : BaseCommandModule
    {
        #region COMMAND_8BALL

        [Command("8ball")]
        [Aliases("8b")]
        [Description("Roll an 8-ball")]
        public Task EightBall(CommandContext ctx, [RemainingText] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return BotServices.SendEmbedAsync(ctx, ":8ball: The almighty 8 ball requests a question", EmbedType.Warning);
            var output = new DiscordEmbedBuilder()
                .WithDescription($":8ball: #{ctx.User.Mention} {EightBallService.GetAnswer()}")
                .WithColor(DiscordColor.Black);
            return ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_8BALL

        #region COMMAND_COINFLIP

        [Command("coinflip")]
        [Aliases("coin", "flip")]
        [Description("Flip a coin")]
        public Task CoinFlip(CommandContext ctx)
        {
            var random = new Random();
            var output = new DiscordEmbedBuilder()
                .WithDescription($"#{ctx.User.Mention} flipped {Formatter.Bold(Convert.ToBoolean(random.Next(0, 2)) ? "Heads" : "Tails")}")
                .WithColor(DiscordColor.Black);
            return ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_COINFLIP

        #region COMMAND_DICEROLL

        [Command("rolldice")]
        [Aliases("dice", "roll")]
        [Description("Roll a die")]
        public Task RollDice(CommandContext ctx)
        {
            var random = new Random();
            var output = new DiscordEmbedBuilder()
                .WithDescription($"#{ctx.User.Mention} rolled a {Formatter.Bold(random.Next(1, 7).ToString())}")
                .WithColor(DiscordColor.Black);
            return ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_DICEROLL

        #region COMMAND_TTS

        [Command("tts")]
        [Description("Sends a Text-to-Speech message.")]
        [RequirePermissions(Permissions.SendTtsMessages)]
        public Task SayTTS(CommandContext ctx, [RemainingText] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return ctx.RespondAsync("I need something to say...");
            return ctx.RespondAsync(Formatter.BlockCode(Formatter.Strip(text)), isTTS: true);
        }

        #endregion COMMAND_TTS

        #region COMMAND_COLOR

        [Command("color")]
        [Description("Get color values corresponding to the inputted RGB")]
        public async Task GetColor(CommandContext ctx, DiscordColor color)
        {
            try
            {
                var output = new DiscordEmbedBuilder()
                    .AddField("RGB:", $"{color.R} {color.G} {color.B}", true)
                    .AddField("HEX:", $"{color.Value:X}", true)
                    .AddField("Decimal:", $"{color.Value}", true)
                    .WithColor(color);
                await ctx.RespondAsync(embed: output.Build());
            }
            catch
            {
                await BotServices.SendEmbedAsync(ctx, ":warning: Unable to retrieve color values, try **.color [0-255] [0-255] [0-255]**", EmbedType.Warning);
            }
        }

        #endregion COMMAND_COLOR

        #region COMMAND_CATFACT

        [Command("catfact")]
        [Aliases("cat")]
        [Description("Get a random cat fact")]
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
        [Description("Get a random cat image")]
        public async Task CatPic(CommandContext ctx)
        {
            var http = new HttpClient();
            var data = await http.GetStringAsync("http://aws.random.cat/meow").ConfigureAwait(false);
            string url = JObject.Parse(data)["file"].ToString();

            if (string.IsNullOrWhiteSpace(url))
                await BotServices.SendEmbedAsync(ctx, "Connection to random.cat failed!", EmbedType.Warning);

            var output = new DiscordEmbedBuilder()
                .WithTitle(":cat: Meow!")
                .WithImageUrl(url)
                .WithColor(DiscordColor.Orange);
            await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_CATPIC

        #region COMMAND_DOGPIC

        [Command("randomdog")]
        [Aliases("woof")]
        [Description("Get a random dog image")]
        public async Task DogPic(CommandContext ctx)
        {
            var data = DogService.GetDogPhotoAsync().Result;
            if (data.status != "success")
                await BotServices.SendEmbedAsync(ctx, "Unable to retrieve a pupper photo :(", EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(":dog: Bork!")
                    .WithImageUrl(data.message)
                    .WithColor(DiscordColor.Brown);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_DOGPIC

        #region COMMAND_TIME

        [Command("time")]
        [Aliases("ti")]
        [Description("Get time for specified location")]
        public async Task GetTime(CommandContext ctx, [RemainingText] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                await BotServices.SendEmbedAsync(ctx, ":warning: A valid location is required! Try **.time Ottawa, CA**", EmbedType.Warning);
            else
            {
                var data = TimeService.GetTimeDataAsync(location).Result;
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Time in " + data.results[0].formatted_address)
                    .WithDescription($":clock1: **{data.time.ToShortTimeString()}** {data.timezone.timeZoneName}")
                    .WithColor(DiscordColor.Cyan);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_TIME

        #region UNUSED

        [Command("shorten"), Hidden]
        [Description("Shorten the inputted URL")]
        public async Task Shorten(CommandContext ctx, [RemainingText] string query)
        {
            if (!Uri.IsWellFormedUriString(query, UriKind.RelativeOrAbsolute)) // && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                await BotServices.SendEmbedAsync(ctx, ":warning: A valid URL link is required!", EmbedType.Warning);
            else
            {
                //var shortenService = new ShortenService();
                //await ctx.RespondAsync(shortenService.shortenUrl(query));
                var shortenService = new Bitly(GlobalVariables.config.BitlyToken);
                var response = shortenService.Shorten(query);
                await ctx.RespondAsync(response.data.custom_short_domain);
            }
        }

        #endregion UNUSED
    }
}