using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Services;
using FlawBOT.Services.Search;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
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
            if (string.IsNullOrWhiteSpace(location))
                await BotServices.SendEmbedAsync(ctx, "A valid location is required! Try **.time Ottawa, CA**", EmbedType.Warning);
            else
            {
                var results = TimeService.GetTimeDataAsync(location).Result;
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Time in " + results.results[0].formatted_address)
                    .WithDescription($":clock1: **{results.time.ToShortTimeString()}** {results.timezone.timeZoneName}")
                    .WithColor(DiscordColor.Cyan);
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
            if (!BotServices.CheckUserInput(ctx, query).Result) return;
            var results = await WeatherService.GetWeatherDataAsync(query);
            if (results.cod == 404)
                await BotServices.SendEmbedAsync(ctx, "Location not found!", EmbedType.Missing);
            else
            {
                Func<double, double> format = WeatherService.CelsiusToFahrenheit;
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Current weather in " + results.name + ", " + results.sys.country)
                    .AddField("Temperature", $"{results.main.temp:F1}°C / {format(results.main.temp):F1}°F", true)
                    .AddField("Conditions", string.Join(", ", results.weather.Select(w => w.main)), true)
                    .AddField("Humidity", $"{results.main.humidity}%", true)
                    .AddField("Wind Speed", $"{results.wind.speed}m/s", true)
                    //.AddField("Temperature (Min/Max)", $"{results.main.tempMin:F1}°C - {results.main.tempMax:F1}°C\n{format(results.main.tempMin):F1}°F - {format(results.main.tempMax):F1}°F", true)
                    .WithUrl("https://openweathermap.org/city/" + results.id)
                    .WithColor(DiscordColor.Cyan);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_WEATHER
    }
}