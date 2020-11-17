using System;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using Newtonsoft.Json.Linq;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class MiscModule : BaseCommandModule
    {
        #region COMMAND_8BALL

        [Command("ask"), Aliases("8b", "8ball", "ball", "8")]
        [Description("Ask an 8-ball a question")]
        public Task EightBall(CommandContext ctx,
            [Description("Question to ask the 8-Ball"), RemainingText]
            string question = "")
        {
            if (string.IsNullOrWhiteSpace(question)) return Task.CompletedTask;
            var output = new DiscordEmbedBuilder()
                .WithDescription(":8ball: " + MiscService.GetRandomAnswer() + " (" + ctx.User.Mention + ")")
                .WithColor(DiscordColor.Black);
            return ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_8BALL

        #region COMMAND_CAT

        [Command("cat"), Aliases("meow", "catfact", "randomcat")]
        [Description("Retrieve a random cat fact")]
        public async Task GetCat(CommandContext ctx)
        {
            var results = MiscService.GetCatFactAsync().Result;
            var output = new DiscordEmbedBuilder()
                .WithFooter($"Fact: {JObject.Parse(results)["fact"]}")
                .WithColor(DiscordColor.Orange);

            var image = MiscService.GetCatPhotoAsync().Result;
            if (!string.IsNullOrWhiteSpace(image))
                output.WithImageUrl(image);

            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_CAT

        #region COMMAND_COINFLIP

        [Command("coinflip"), Aliases("coin", "flip")]
        [Description("Flip a coin")]
        public Task CoinFlip(CommandContext ctx)
        {
            var random = new Random();
            var output = new DiscordEmbedBuilder()
                .WithDescription(ctx.User.Username + " flipped a coin and got " +
                                 Formatter.Bold(Convert.ToBoolean(random.Next(0, 2)) ? "Heads" : "Tails"))
                .WithColor(SharedData.DefaultColor);
            return ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_COINFLIP

        #region COMMAND_DICEROLL

        [Command("diceroll"), Aliases("dice", "roll", "rolldice", "die")]
        [Description("Roll a six-sided die")]
        public Task RollDice(CommandContext ctx)
        {
            var random = new Random();
            var output = new DiscordEmbedBuilder()
                .WithDescription(ctx.User.Username + " rolled a die and got " +
                                 Formatter.Bold(random.Next(1, 7).ToString()))
                .WithColor(SharedData.DefaultColor);
            return ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_DICEROLL

        #region COMMAND_DOG

        [Command("randomdog"), Aliases("woof", "dog", "bark")]
        [Description("Retrieve a random dog photo")]
        public async Task GetDog(CommandContext ctx)
        {
            var results = MiscService.GetDogPhotoAsync().Result;
            if (results.Status != "success")
            {
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_DOG_PHOTO, EmbedType.Warning).ConfigureAwait(false);
                return;
            }

            var output = new DiscordEmbedBuilder()
                .WithImageUrl(results.Message)
                .WithColor(DiscordColor.Brown);
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_DOG

        #region COMMAND_HELLO

        [Command("hello"), Aliases("hi", "howdy")]
        [Description("Welcome another user to the server")]
        public async Task Greet(CommandContext ctx,
            [Description("User to say hello to"), RemainingText]
            DiscordMember member)
        {
            if (member is null)
                await ctx.RespondAsync(":wave: Hello, " + ctx.User.Mention).ConfigureAwait(false);
            else
                await ctx.RespondAsync(":wave: Welcome " + member.Mention + " to " + ctx.Guild.Name +
                                       ". Enjoy your stay!").ConfigureAwait(false);
        }

        #endregion COMMAND_HELLO

        #region COMMAND_IP

        [Command("ip"), Aliases("ipstack", "track")]
        [Description("Retrieve IP address geolocation information")]
        public async Task IpTrack(CommandContext ctx,
            [Description("IP Address")] string address)
        {
            if (string.IsNullOrWhiteSpace(address) || !IPAddress.TryParse(address, out var ip))
            {
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_INVALID_IP, EmbedType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            var results = MiscService.GetIpLocationAsync(ip).Result;
            if (results.Type == null)
            {
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_LOCATION, EmbedType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            var output = new DiscordEmbedBuilder()
                .WithTitle($"{results.City}, {results.Region}, {results.Country}")
                .WithDescription($"Coordinates: {results.Latitude}°N, {results.Longitude}°W")
                .WithUrl(string.Format(Resources.URL_GOOGLE_MAPS, results.Latitude, results.Longitude))
                .WithColor(new DiscordColor("#4d2f63"));
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_IP
    }
}