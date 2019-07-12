using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class GoogleService : HttpHandler
    {
        private static readonly string base_url = "http://api.openweathermap.org/data/2.5/weather?";
        private static readonly string geocode_url = "https://maps.googleapis.com/maps/api/geocode/json?address=";
        private static readonly string timezone_url = "https://maps.googleapis.com/maps/api/timezone/json?location=";

        public static async Task<TimeData> GetTimeDataAsync(string query)
        {
            try
            {
                var results = GetLocationGeoData(query.Replace(" ", "")).Result;
                if (results == null)
                    return null;
                else
                {
                    var latitude = results.Results[0].Geometry.Location.Latitude;
                    var longitude = results.Results[0].Geometry.Location.Longitude;
                    var currentSeconds = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    var url = timezone_url + latitude + "," + longitude + $"&timestamp={currentSeconds}&key={TokenHandler.Tokens.GoogleToken}";
                    var timeResource = await _http.GetStringAsync(url);
                    results.Timezone = JsonConvert.DeserializeObject<TimeData.TimeZoneResult>(timeResource);
                    results.Time = DateTime.UtcNow.AddSeconds(results.Timezone.dstOffset + results.Timezone.rawOffset);
                }
                return results;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<WeatherData> GetWeatherDataAsync(string query)
        {
            _http.DefaultRequestHeaders.Clear();
            var results = await _http.GetStringAsync(base_url + $"q={query}&appid=42cd627dd60debf25a5739e50a217d74&units=metric");
            return JsonConvert.DeserializeObject<WeatherData>(results);
        }

        public static double CelsiusToFahrenheit(double cel)
        {
            return cel * 1.8f + 32;
        }

        public async static Task<TimeData> GetLocationGeoData(string query)
        {
            _http.DefaultRequestHeaders.Clear();
            var result = await _http.GetStringAsync(geocode_url + query + $"&key={TokenHandler.Tokens.GoogleToken}");
            var results = JsonConvert.DeserializeObject<TimeData>(result);
            if (results.status == "OK")
                return results;
            else
                return null;
        }
    }
}