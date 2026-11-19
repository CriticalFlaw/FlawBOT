using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Weather;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class WeatherService : HttpHandler
    {
        public static async Task<DiscordEmbed> GetWeatherDataAsync(string token, string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(query)) return null;
                var url = string.Format(Resources.URL_Weather, Uri.EscapeDataString(token), Uri.EscapeDataString(query.Trim()));
                var response = await Http.GetStringAsync(url).ConfigureAwait(false);
                return BuildWeatherEmbed(JsonConvert.DeserializeObject<WeatherData>(response));
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed BuildWeatherEmbed(WeatherData results)
        {
            // Weatherstack reports errors (unknown location, bad key) as a 200 response without location data.
            if (results?.Location is null || results.Current is null) return null;
            return new DiscordEmbedBuilder()
                .WithTitle(results.Location.Name + ", " + results.Location.Country)
                .WithDescription($"Currently: {results.Current.Descriptions?.FirstOrDefault() ?? EmbedExtensions.Unknown}")
                .AddSafeField(":thermometer: Temperature", $"{results.Current.Temperature:F1}°C / {CelsiusToFahrenheit(results.Current.Temperature):F1}°F", true)
                .AddSafeField(":droplet: Humidity", $"{results.Current.Humidity}%", true)
                .WithFooter($"Local Time: {results.Location.LocalTime}")
                .WithColor(Program.Settings.DefaultColor)
                .Build();
        }

        public static double CelsiusToFahrenheit(double cel)
        {
            return cel * 1.8 + 32;
        }
    }
}
