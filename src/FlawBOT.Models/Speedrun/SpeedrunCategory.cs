using Newtonsoft.Json;

namespace FlawBOT.Models.Speedrun
{
    public class SpeedrunCategory
    {
        [JsonProperty("data")]
        public List<CategoryData> Data { get; set; }
    }
}