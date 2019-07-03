using FlawBOT.Common;
using FlawBOT.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class TimeService
    {
        private static readonly string geocode_url = "https://maps.googleapis.com/maps/api/geocode/json?address=";
        private static readonly string timezone_url = "https://maps.googleapis.com/maps/api/timezone/json?location=";
        private static readonly HttpClient http = new HttpClient();

        public static async Task<TimeData> GetTimeDataAsync(string query)
        {
            try
            {
                http.DefaultRequestHeaders.Clear();
                var token = SharedData.Tokens.GoogleToken;
                var result = await http.GetStringAsync(geocode_url + query.Replace(" ", "") + $"&key={token}");
                var results = JsonConvert.DeserializeObject<TimeData>(result);
                if (results.status != "OK")
                    return null;
                else
                {
                    var currentSeconds = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    var url = timezone_url + results.Results[0].geometry.location.lat + $",{results.Results[0].geometry.location.lng}&timestamp={currentSeconds}&key={token}";
                    var timeResource = await http.GetStringAsync(url);
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
    }
}