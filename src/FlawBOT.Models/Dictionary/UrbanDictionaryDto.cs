using Newtonsoft.Json;

namespace FlawBOT.Models.Dictionary
{
    public class UrbanDictionaryDto
    {
        [JsonProperty("definition")]
        public string Definition { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("example")]
        public string Example { get; set; }

        [JsonProperty("thumbs_up")]
        public int ThumbsUp { get; set; }

        [JsonProperty("thumbs_down")]
        public int ThumbsDown { get; set; }
    }
}