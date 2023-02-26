﻿using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Modules.Bot;
using FlawBOT.Properties;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Weather
{
    public class WeatherModule : ApplicationCommandModule
    {
        #region COMMAND_WEATHER

        [SlashCommand("weather", "Retrieve the time and weather for specified location.")]
        public async Task Weather(InteractionContext ctx, [Option("query", "Location to get time and weather data from.")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await WeatherService.GetWeatherDataAsync(Program.Settings.Tokens.WeatherToken, query).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_LOCATION, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            Func<double, double> format = WeatherService.CelsiusToFahrenheit;
            var output = new DiscordEmbedBuilder()
                .WithDescription("Weather in " + results.Location.Name + ", " + results.Location.Country)
                .AddField(":partly_sunny: Currently", results.Current.Descriptions.FirstOrDefault(), true)
                .AddField(":thermometer: Temperature", $"{results.Current.Temperature:F1}°C / {format(results.Current.Temperature):F1}°F", true)
                .AddField(":droplet: Humidity", $"{results.Current.Humidity}%", true)
                .AddField(":clock1: Local Time", results.Location.LocalTime, true)
                .WithColor(Program.Settings.DefaultColor);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_WEATHER
    }
}