using System;
using System.Linq;
using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Services
{
    public class GoogleService : HttpHandler
    {
        public static async Task<TimeData> GetTimeDataAsync(string query)
        {
            try
            {
                var results = GetLocationGeoData(query.Replace(" ", "")).Result;
                if (results?.Results is null) return null;
                var latitude = results.Results.FirstOrDefault().Geometry.Location.Latitude;
                var longitude = results.Results.FirstOrDefault().Geometry.Location.Longitude;
                var currentSeconds = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var timeResource = await Http
                    .GetStringAsync(string.Format(Resources.API_Google_Time, latitude, longitude, currentSeconds,
                        TokenHandler.Tokens.GoogleToken))
                    .ConfigureAwait(false);
                results.Timezone = JsonConvert.DeserializeObject<TimeData.TimeZoneResult>(timeResource);
                results.Time = DateTime.UtcNow.AddSeconds(results.Timezone.DstOffset + results.Timezone.RawOffset);
                return results;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<WeatherData> GetWeatherDataAsync(string query)
        {
            try
            {
                var results = await Http
                    .GetStringAsync(string.Format(Resources.API_Google_Weather, query)).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<WeatherData>(results);
            }
            catch
            {
                return null;
            }
        }

        private static async Task<TimeData> GetLocationGeoData(string query)
        {
            Http.DefaultRequestHeaders.Clear();
            var result = await Http
                .GetStringAsync(string.Format(Resources.API_Google_Geo, query, TokenHandler.Tokens.GoogleToken))
                .ConfigureAwait(false);
            var results = JsonConvert.DeserializeObject<TimeData>(result);
            return results.Status == "OK" ? results : null;
        }

        public static async Task<NewsData> GetNewsDataAsync(string query = "")
        {
            var results = await Http
                .GetStringAsync(string.Format(Resources.API_News, query, TokenHandler.Tokens.NewsToken))
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<NewsData>(results);
        }

        public static double CelsiusToFahrenheit(double cel)
        {
            return cel * 1.8f + 32;
        }
    }
}