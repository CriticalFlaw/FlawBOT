using Newtonsoft.Json;

namespace FlawBOT.Models.Weather
{
    public class Location
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; }

        [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
        public string Region { get; set; }

        [JsonProperty("lat", NullValueHandling = NullValueHandling.Ignore)]
        public string Latitude { get; set; }

        [JsonProperty("lon", NullValueHandling = NullValueHandling.Ignore)]
        public string Longitude { get; set; }

        [JsonProperty("timezone_id", NullValueHandling = NullValueHandling.Ignore)]
        public string Timezone { get; set; }

        [JsonProperty("localtime", NullValueHandling = NullValueHandling.Ignore)]
        public string LocalTime { get; set; }

        [JsonProperty("localtime_epoch", NullValueHandling = NullValueHandling.Ignore)]
        public int LocalTimeEpoch { get; set; }

        [JsonProperty("utc_offset", NullValueHandling = NullValueHandling.Ignore)]
        public string UtcOffset { get; set; }
    }
}