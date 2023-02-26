using Newtonsoft.Json;

namespace FlawBOT.Models.Speedrun
{
    public class CategoryData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("weblink")]
        public Uri Weblink { get; set; }

        [JsonIgnore]
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("rules")]
        public string Rules { get; set; }

        [JsonProperty("players")]
        public Players Players { get; set; }

        [JsonIgnore]
        [JsonProperty("miscellaneous")]
        public bool Miscellaneous { get; set; }

        [JsonProperty("links")]
        public List<Link> Links { get; set; }
    }
}