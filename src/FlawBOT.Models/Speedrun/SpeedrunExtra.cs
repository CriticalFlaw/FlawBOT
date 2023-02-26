using Newtonsoft.Json;

namespace FlawBOT.Models.Speedrun
{
    public class SpeedrunExtra
    {
        [JsonProperty("data")]
        public ExtraData Data { get; set; }
    }
}