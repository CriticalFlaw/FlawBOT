using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class GoogleService : HttpHandler
    {
        public static async Task<TimeData> GetTimeDataAsync(string query)
        {
            try
            {
                var results = GetLocationGeoData(query.Replace(" ", "")).Result;
                if (results is null) return null;
                var latitude = results.Results[0].Geometry.Location.Latitude;
                var longitude = results.Results[0].Geometry.Location.Longitude;
                var currentSeconds = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var timeResource = await _http.GetStringAsync(Resources.API_Google_Time + "?location=" + latitude + "," + longitude + "&timestamp=" + currentSeconds + "&key=" + TokenHandler.Tokens.GoogleToken).ConfigureAwait(false);
                results.Timezone = JsonConvert.DeserializeObject<TimeData.TimeZoneResult>(timeResource);
                results.Time = DateTime.UtcNow.AddSeconds(results.Timezone.dstOffset + results.Timezone.rawOffset);
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
                var results = await _http.GetStringAsync(Resources.API_Google_Weather + "?q=" + query + "&appid=42cd627dd60debf25a5739e50a217d74&units=metric").ConfigureAwait(false);
                return JsonConvert.DeserializeObject<WeatherData>(results);
            }
            catch
            {
                return null;
            }
        }

        public async static Task<TimeData> GetLocationGeoData(string query)
        {
            _http.DefaultRequestHeaders.Clear();
            var result = await _http.GetStringAsync(Resources.API_Google_Geo + "?address=" + query + "&key=" + TokenHandler.Tokens.GoogleToken).ConfigureAwait(false);
            var results = JsonConvert.DeserializeObject<TimeData>(result);
            return (results.status == "OK") ? results : null;
        }

        public static async Task<IPLocationData> GetIPLocationAsync(IPAddress query)
        {
            var result = await _http.GetStringAsync(Resources.API_IPLocation + query.ToString()).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IPLocationData>(result);
        }

        public static async Task<NewsData> GetNewsDataAsync(string query = "")
        {
            var results = await _http.GetStringAsync(Resources.API_News + "&q=" + query + "&apiKey=" + TokenHandler.Tokens.NewsToken).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<NewsData>(results);
        }

        public static double CelsiusToFahrenheit(double cel)
        {
            return (cel * 1.8f) + 32;
        }
    }
}