using FlawBOT.Common;
using FlawBOT.Models.Weather;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class WeatherService : HttpHandler
    {
        public static async Task<WeatherData> GetWeatherDataAsync(string token, string query)
        {
            try
            {
                var results = await Http
                    .GetStringAsync(string.Format(Resources.URL_Weather, token, query))
                    .ConfigureAwait(false);
                return JsonConvert.DeserializeObject<WeatherData>(results);
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