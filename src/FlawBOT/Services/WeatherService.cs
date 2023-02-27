using FlawBOT.Common;
using FlawBOT.Models.Weather;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class WeatherService : HttpHandler
    {
        public static async Task<WeatherData> GetWeatherDataAsync(string token, string query)
        {
            try
            {
                query = string.Format(Resources.URL_Weather, token, query.Trim());
                var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<WeatherData>(response);
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