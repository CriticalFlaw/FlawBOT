using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Weather;
using FlawBOT.Properties;
using Newtonsoft.Json;
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
                if (string.IsNullOrWhiteSpace(query)) return null;
                query = string.Format(Resources.URL_Weather, token, query.Trim());
                var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                var results = JsonConvert.DeserializeObject<WeatherData>(response);

                var output = new DiscordEmbedBuilder()
                    .WithDescription("Weather in " + results.Location.Name + ", " + results.Location.Country)
                    .AddField(":partly_sunny: Currently", results.Current.Descriptions.FirstOrDefault(), true)
                    .AddField(":thermometer: Temperature", $"{results.Current.Temperature:F1}°C / {CelsiusToFahrenheit(results.Current.Temperature):F1}°F", true)
                    .AddField(":droplet: Humidity", $"{results.Current.Humidity}%", true)
                    .AddField(":clock1: Local Time", results.Location.LocalTime, true)
                    .WithColor(Program.Settings.DefaultColor);
                return output.Build();
            }
            catch
            {
                return null;
            }
        }

        public static double CelsiusToFahrenheit(double cel)
        {
            return cel * 1.8f + 32;
        }
    }
}