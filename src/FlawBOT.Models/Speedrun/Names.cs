using Newtonsoft.Json;

namespace FlawBOT.Models.Speedrun
{
    public class Names
    {
        [JsonProperty("international")]
        public string International { get; set; }

        [JsonIgnore]
        [JsonProperty("japanese")]
        public string Japanese { get; set; }

        [JsonIgnore]
        [JsonProperty("twitch")]
        public string Twitch { get; set; }
    }
}