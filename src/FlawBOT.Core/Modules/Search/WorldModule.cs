using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

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
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_INVALID_IP, EmbedType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            var results = WorldService.GetIpLocationAsync(ip).Result;
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
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_LOCATION, EmbedType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            Func<double, double> format = WorldService.CelsiusToFahrenheit;
            var output = new DiscordEmbedBuilder()
                .WithTitle("Current weather in " + results.Location.Name + ", " + results.Location.Region + ", " + results.Location.Country)
                .AddField("Temperature",
                    $"{results.Current.Temperature:F1}°C / {format(results.Current.Temperature):F1}°F", true)
                .AddField("Humidity", $"{results.Current.Humidity}%", true)
                .AddField("Wind Speed", $"{results.Current.Wind_Speed}m/s", true)
                .AddField("Local Time", results.Location.LocalTime, true)
                .WithThumbnail(results.Current.Icons.FirstOrDefault())
                .WithColor(SharedData.DefaultColor);
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }
    }

    #endregion COMMAND_WEATHER
}