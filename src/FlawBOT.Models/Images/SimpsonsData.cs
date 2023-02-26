using Newtonsoft.Json;

namespace FlawBOT.Models.Images
{
    public class SimpsonsData
    {
        [JsonProperty("Episode")]
        public Episode Episode { get; set; }

        [JsonProperty("Frame")]
        public Frame Frame { get; set; }
    }
}