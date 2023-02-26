using Newtonsoft.Json;

namespace FlawBOT.Models.Images
{
    public class Frame
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Episode")]
        public string Episode { get; set; }

        [JsonProperty("Timestamp")]
        public int Timestamp { get; set; }
    }
}