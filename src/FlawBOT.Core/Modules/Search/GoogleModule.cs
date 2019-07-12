using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
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
                await BotServices.SendEmbedAsync(ctx, "Location not found!", EmbedType.Missing);
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
    }
}