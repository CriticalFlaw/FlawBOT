using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class WeatherModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns weather information for a given location.
        /// </summary>
        [SlashCommand("weather", "Returns weather information for a given location.")]
        public async Task Weather(InteractionContext ctx, [Option("query", "Location to get time and weather data from.")] string query)
        {
            var output = await WeatherService.GetWeatherDataAsync(Program.Settings.Tokens.WeatherToken, query).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_LOCATION, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}