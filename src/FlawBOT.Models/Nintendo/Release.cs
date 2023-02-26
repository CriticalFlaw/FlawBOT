using Newtonsoft.Json;

namespace FlawBOT.Models.Nintendo
{
    public class Release
    {
        [JsonProperty("au")]
        public string Australian { get; set; }

        [JsonProperty("eu")]
        public string European { get; set; }

        [JsonProperty("jp")]
        public string Japanese { get; set; }

        [JsonProperty("na")]
        public string American { get; set; }
    }
}