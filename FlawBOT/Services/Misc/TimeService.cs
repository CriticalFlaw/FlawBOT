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
                var service = new BotServices();
                var token = GlobalVariables.config.GoogleToken;
                var result = await http.GetStringAsync(geocode_url + query.Replace(" ", "") + $"&key={token}");
                var data = JsonConvert.DeserializeObject<TimeData>(result);
                if (data.status != "OK")
                    return null;
                else
                {
                    var currentSeconds = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    var url = timezone_url + data.results[0].geometry.location.lat + $",{data.results[0].geometry.location.lng}&timestamp={currentSeconds}&key={token}";
                    var timeResource = await http.GetStringAsync(url);
                    data.timezone = JsonConvert.DeserializeObject<TimeData.TimeZoneResult>(timeResource);
                    data.time = DateTime.UtcNow.AddSeconds(data.timezone.dstOffset + data.timezone.rawOffset);
                }
                return data;
            }
            catch
            {
                return null;
            }
        }
    }
}