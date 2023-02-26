using Newtonsoft.Json;

namespace FlawBOT.Models.Weather
{
    public class WeatherData
    {
        [JsonProperty("request", NullValueHandling = NullValueHandling.Ignore)]
        public Request Request { get; set; }

        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public Location Location { get; set; }

        [JsonProperty("current", NullValueHandling = NullValueHandling.Ignore)]
        public Current Current { get; set; }
    }
}