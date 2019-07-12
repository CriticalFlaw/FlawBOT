using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    public class DogData
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}