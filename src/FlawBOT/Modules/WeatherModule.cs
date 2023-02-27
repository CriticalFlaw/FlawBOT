using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class WeatherModule : ApplicationCommandModule
    {
        [SlashCommand("weather", "Retrieve the time and weather for specified location.")]
        public async Task Weather(InteractionContext ctx, [Option("query", "Location to get time and weather data from.")] string query)
        {
            var results = await WeatherService.GetWeatherDataAsync(Program.Settings.Tokens.WeatherToken, query).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_LOCATION, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            var output = new DiscordEmbedBuilder()
                .WithDescription("Weather in " + results.Location.Name + ", " + results.Location.Country)
                .AddField(":partly_sunny: Currently", results.Current.Descriptions.FirstOrDefault(), true)
                .AddField(":thermometer: Temperature", $"{results.Current.Temperature:F1}°C / {WeatherService.CelsiusToFahrenheit(results.Current.Temperature):F1}°F", true)
                .AddField(":droplet: Humidity", $"{results.Current.Humidity}%", true)
                .AddField(":clock1: Local Time", results.Location.LocalTime, true)
                .WithColor(Program.Settings.DefaultColor);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }
    }
}