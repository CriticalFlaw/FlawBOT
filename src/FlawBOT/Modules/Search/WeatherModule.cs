using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using FlawBOT.Services.Lookup;

namespace FlawBOT.Modules.Search
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class WeatherModule : BaseCommandModule
    {
        #region COMMAND_WEATHER

        [Command("weather")]
        [Aliases("time")]
        [Description("Retrieve the time and weather for specified location.")]
        public async Task Weather(CommandContext ctx,
            [Description("Location to get time and weather data from.")] [RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            await ctx.TriggerTypingAsync();
            var results = await WeatherService.GetWeatherDataAsync(Program.Settings.Tokens.WeatherToken, query)
                .ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_LOCATION, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            Func<double, double> format = WeatherService.CelsiusToFahrenheit;
            var output = new DiscordEmbedBuilder()
                .WithDescription("Weather in " + results.Location.Name + ", " + results.Location.Country)
                .AddField(":partly_sunny: Currently", results.Current.Descriptions.FirstOrDefault(), true)
                .AddField(":thermometer: Temperature",
                    $"{results.Current.Temperature:F1}°C / {format(results.Current.Temperature):F1}°F", true)
                .AddField(":droplet: Humidity", $"{results.Current.Humidity}%", true)
                .AddField(":clock1: Local Time", results.Location.LocalTime, true)
                .WithColor(Program.Settings.DefaultColor);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_WEATHER
    }
}