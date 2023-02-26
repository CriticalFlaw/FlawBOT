using Newtonsoft.Json;

namespace FlawBOT.Models.Speedrun
{
    public class SpeedrunGame
    {
        [JsonProperty("data")]
        public List<Data> Data { get; set; }
    }
}