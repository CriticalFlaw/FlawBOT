using Newtonsoft.Json;

namespace FlawBOT.Models.Speedrun
{
    public class Link
    {
        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("uri")]
        public string Url { get; set; }
    }
}