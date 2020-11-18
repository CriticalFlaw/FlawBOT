using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Services
{
    public class WorldService : HttpHandler
    {
        public static async Task<IpStack> GetIpLocationAsync(IPAddress query)
        {
            var result = await Http.GetStringAsync(string.Format(Resources.API_IPLocation, query))
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IpStack>(result);
        }

        public static async Task<WeatherData> GetWeatherDataAsync(string query)
        {
            try
            {
                var results = await Http.GetStringAsync(string.Format(Resources.API_Weather, TokenHandler.Tokens.WeatherToken, query)).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<WeatherData>(results);
            }
            catch
            {
                return null;
            }
        }

        public static double CelsiusToFahrenheit(double cel)
        {
            return (cel * 1.8f) + 32;
        }
    }
}