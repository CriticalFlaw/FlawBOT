using System.Net;
using System.Threading.Tasks;
using FlawBOT.Common;
using FlawBOT.Models;
using FlawBOT.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Services
{
    public class WorldService : HttpHandler
    {
        public static async Task<IpStack> GetIpLocationAsync(IPAddress query)
        {
            var result = await Http.GetStringAsync(string.Format(Resources.URL_IPStack, query))
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IpStack>(result);
        }

        public static async Task<WeatherData> GetWeatherDataAsync(string query)
        {
            try
            {
                var results = await Http
                    .GetStringAsync(string.Format(Resources.URL_Weather, Program.Settings.Tokens.WeatherToken, query))
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