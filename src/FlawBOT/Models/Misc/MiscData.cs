using Newtonsoft.Json;

namespace FlawBOT.Models
{
    public class DogData
    {
        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }
    }
}