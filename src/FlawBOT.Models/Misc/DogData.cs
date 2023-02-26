using Newtonsoft.Json;

namespace FlawBOT.Models.Misc
{
    public class DogData
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}