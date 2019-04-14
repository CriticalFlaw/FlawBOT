using FlawBOT.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services.Search
{
    public class WeatherService
    {
        private static readonly string base_url = "http://api.openweathermap.org/data/2.5/weather?";
        private static readonly HttpClient http = new HttpClient();

        public static async Task<WeatherData> GetWeatherDataAsync(string query)
        {
            http.DefaultRequestHeaders.Clear();
            var results = await http.GetStringAsync(base_url + $"q={query}&appid=42cd627dd60debf25a5739e50a217d74&units=metric");
            return JsonConvert.DeserializeObject<WeatherData>(results);
        }

        public static double CelsiusToFahrenheit(double cel)
        {
            return cel * 1.8f + 32;
        }
    }
}