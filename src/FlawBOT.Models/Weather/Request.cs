using Newtonsoft.Json;

namespace FlawBOT.Models.Weather
{
    public class Request
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
        public string Query { get; set; }

        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }

        [JsonProperty("unit", NullValueHandling = NullValueHandling.Ignore)]
        public string Unit { get; set; }
    }
}