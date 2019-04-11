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
    public class WeatherModule : BaseCommandModule
    {
        #region COMMAND_WEATHER

        [Command("weather")]
        [Description("Get weather information for specified location")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Weather(CommandContext ctx, [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(ctx, query).Result) return;
            var data = await WeatherService.GetWeatherDataAsync(query);
            if (data.cod == 404)
                await BotServices.SendEmbedAsync(ctx, ":mag: Location not found!", EmbedType.Warning);
            else
            {
                Func<double, double> format = WeatherService.CelsiusToFahrenheit;
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Current weather in " + data.name + ", " + data.sys.country)
                    .AddField("Temperature", $"{data.main.temp:F1}°C / {format(data.main.temp):F1}°F", true)
                    .AddField("Conditions", string.Join(", ", data.weather.Select(w => w.main)), true)
                    .AddField("Humidity", $"{data.main.humidity}%", true)
                    .AddField("Wind Speed", $"{data.wind.speed}m/s", true)
                    .AddField("Temperature (Min/Max)", $"{data.main.tempMin:F1}°C - {data.main.tempMax:F1}°C\n{format(data.main.tempMin):F1}°F - {format(data.main.tempMax):F1}°F", true)
                    .WithUrl("https://openweathermap.org/city/" + data.id)
                    .WithColor(DiscordColor.Cyan);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_WEATHER
    }
}