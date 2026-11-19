using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class WeatherModule : ApplicationCommandModule
    {
        [SlashCommand("weather", "Returns weather information for a given location.")]
        public async Task Weather(InteractionContext ctx, [Option("query", "Location to get time and weather data from.")] string query)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await WeatherService.GetWeatherDataAsync(Program.Settings.Tokens.WeatherToken, query).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_LOCATION, ResponseType.Missing).ConfigureAwait(false);
        }
    }
}
