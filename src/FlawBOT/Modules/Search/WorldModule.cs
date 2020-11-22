using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class WorldModule : BaseCommandModule
    {
        #region COMMAND_IP

        [Command("ip"), Aliases("ipstack", "track")]
        [Description("Retrieve IP address geolocation information")]
        public async Task IpTrack(CommandContext ctx,
            [Description("IP Address")] string address)
        {
            if (string.IsNullOrWhiteSpace(address) || !IPAddress.TryParse(address, out var ip))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_INVALID_IP, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            var results = WorldService.GetIpLocationAsync(ip).Result;
            if (results.Type == null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_LOCATION, ResponseType.Warning)
                    .ConfigureAwait(false);
                return;
            }

            var output = new DiscordEmbedBuilder()
                .WithTitle($"{results.City}, {results.Region}, {results.Country}")
                .WithDescription($"Coordinates: {results.Latitude}°N, {results.Longitude}°W")
                .WithUrl(string.Format(Resources.URL_Google_Maps, results.Latitude, results.Longitude))
                .WithColor(new DiscordColor("#4d2f63"));
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_IP

        #region COMMAND_WEATHER

        [Command("weather"), Aliases("time", "clock")]
        [Description("Retrieve the time and weather for specified location")]
        public async Task Weather(CommandContext ctx,
            [Description("Location from which to retrieve data"), RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await WorldService.GetWeatherDataAsync(query).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_LOCATION, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            Func<double, double> format = WorldService.CelsiusToFahrenheit;
            var output = new DiscordEmbedBuilder()
                .WithDescription("Weather in " + results.Location.Name + ", " + results.Location.Country)
                .AddField(":partly_sunny: Currently", results.Current.Descriptions.FirstOrDefault(), true)
                .AddField(":thermometer: Temperature",
                    $"{results.Current.Temperature:F1}°C / {format(results.Current.Temperature):F1}°F", true)
                .AddField(":droplet: Humidity", $"{results.Current.Humidity}%", true)
                .AddField(":clock1: Local Time", results.Location.LocalTime, true)
                .WithColor(SharedData.DefaultColor);
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }
    }

    #endregion COMMAND_WEATHER
}