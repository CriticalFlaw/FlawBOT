using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules
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
                return BotServices.SendEmbedAsync(ctx, Resources.ERR_8BALL_QUESTION, EmbedType.Warning);
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
            var results = CatService.GetCatFactAsync().Result;
            var output = new DiscordEmbedBuilder()
                .WithTitle($":cat: {JObject.Parse(results)["fact"]}")
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
            var results = CatService.GetCatPhotoAsync().Result;
            if (string.IsNullOrWhiteSpace(results))
                await BotServices.SendEmbedAsync(ctx, "Connection to random.cat failed!", EmbedType.Warning);
            var output = new DiscordEmbedBuilder()
                .WithTitle(":cat: Meow!")
                .WithImageUrl(results)
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
                .WithColor(SharedData.DefaultColor);
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
            if (!regex.Success)
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_COLOR_INVALID, EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .AddField("HEX:", $"{color.Value:X}", true)
                    .AddField("RGB:", $"{color.R} {color.G} {color.B}", true)
                    .AddField("Decimal:", color.Value.ToString(), true)
                    .WithColor(color);
                await ctx.RespondAsync(embed: output.Build());
            }
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
                .WithColor(SharedData.DefaultColor);
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
            if (results.Status != "success")
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_DOG_PHOTO, EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(":dog: Woof!")
                    .WithImageUrl(results.Message)
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
            if (member is null)
                await ctx.RespondAsync($":wave: Hello, " + ctx.User.Mention);
            else
                await ctx.RespondAsync($":wave: Welcome " + member.Mention + " to " + ctx.Guild.Name + ". Enjoy your stay!");
        }

        #endregion COMMAND_HELLO

        #region COMMAND_IP

        [Command("ip")]
        [Aliases("ipstack")]
        [Description("Retrieve IP address geolocation information")]
        public async Task IPLocation(CommandContext ctx,
            [Description("IP Address")] string address)
        {
            if (string.IsNullOrWhiteSpace(address) || !IPAddress.TryParse(address, out IPAddress ip)) return;
            var results = GoogleService.GetIPLocationAsync(ip).Result;
            if (results.Status != "success")
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_LOCATION, EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .AddField("Location", $"{results.City}, {results.Region}, {results.Country}", true)
                    .AddField("ISP", results.ISP, true)
                    .AddField("Longitude", results.Longitude.ToString(), true)
                    .AddField("Latitude", results.Latitude.ToString(), true)
                    .WithFooter($"IP: {results.Query}")
                    .WithColor(new DiscordColor("#4d2f63"));
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_IP

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