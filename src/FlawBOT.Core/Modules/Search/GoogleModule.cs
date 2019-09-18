using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Common;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class GoogleModule : BaseCommandModule
    {
        #region COMMAND_TIME

        [Command("time")]
        [Aliases("clock")]
        [Description("Retrieve the time for specified location")]
        public async Task GetTime(CommandContext ctx,
            [Description("Location to retrieve time data from")] [RemainingText] string location)
        {
            if (!BotServices.CheckUserInput(location)) return;
            var results = GoogleService.GetTimeDataAsync(location).Result;
            if (results.status != "OK")
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(":clock1: Time in " + results.Results[0].FormattedAddress)
                    .WithDescription(Formatter.Bold(results.Time.ToShortTimeString()) + " " + results.Timezone.timeZoneName)
                    .WithColor(SharedData.DefaultColor);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_TIME

        #region COMMAND_WEATHER

        [Command("weather")]
        [Description("Retrieve the weather for specified location")]
        public async Task Weather(CommandContext ctx,
            [Description("Location to retrieve weather data from")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = await GoogleService.GetWeatherDataAsync(query);
            if (results.COD == 404)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_LOCATION, EmbedType.Missing);
            else
            {
                Func<double, double> format = GoogleService.CelsiusToFahrenheit;
                var output = new DiscordEmbedBuilder()
                    .WithTitle(":partly_sunny: Current weather in " + results.Name + ", " + results.Sys.Country)
                    .AddField("Temperature", $"{results.Main.Temperature:F1}°C / {format(results.Main.Temperature):F1}°F", true)
                    .AddField("Conditions", string.Join(", ", results.Weather.Select(w => w.Main)), true)
                    .AddField("Humidity", $"{results.Main.Humidity}%", true)
                    .AddField("Wind Speed", $"{results.Wind.Speed}m/s", true)
                    .WithUrl("https://openweathermap.org/city/" + results.ID)
                    .WithColor(SharedData.DefaultColor);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_WEATHER

        #region COMMAND_NEWS

        [Command("news")]
        [Description("Retrieve the latest news from Google")]
        public async Task News(CommandContext ctx,
            [Description("Article topic to find on Google News")] [RemainingText] string query)
        {
            var results = await GoogleService.GetNewsDataAsync(query);
            if (results.Status != "ok")
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing);
            else
            {
                foreach (var article in results.Articles)
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(article.Title)
                        .WithTimestamp(article.PublishDate)
                        .WithDescription(article.Description.Length < 500 ? article.Description : article.Description.Take(500) + "...")
                        .WithUrl(article.Url)
                        .WithImageUrl(article.UrlImage)
                        .WithFooter("Type 'next' within 10 seconds for the next article. Powered by News API")
                        .WithColor(new DiscordColor("#253B80"));
                    var message = await ctx.RespondAsync(embed: output.Build());

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
                    if (interactivity.Result == null) break;
                    await BotServices.RemoveMessage(interactivity.Result);
                    await BotServices.RemoveMessage(message);
                }
            }
        }

        #endregion COMMAND_NEWS
    }
}