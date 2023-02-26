using Newtonsoft.Json;

namespace FlawBOT.Models.Speedrun
{
    public class ExtraData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonIgnore]
        [JsonProperty("released")]
        public int? Released { get; set; }

        [JsonProperty("links")]
        public List<Link> Links { get; set; }
    }
}