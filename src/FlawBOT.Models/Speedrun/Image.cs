using Newtonsoft.Json;

namespace FlawBOT.Models.Speedrun
{
    public class Image
    {
        [JsonProperty("uri")]
        public string Url { get; set; }

        [JsonIgnore]
        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonIgnore]
        [JsonProperty("height")]
        public int Height { get; set; }
    }
}