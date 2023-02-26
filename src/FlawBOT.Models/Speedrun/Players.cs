using Newtonsoft.Json;

namespace FlawBOT.Models.Speedrun
{
    public class Players
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }
}