using Newtonsoft.Json;

namespace FlawBOT.Models.Speedrun
{
    public class Assets
    {
        [JsonProperty("logo")]
        public Image Logo { get; set; }

        [JsonProperty("cover-small")]
        public Image CoverSmall { get; set; }

        [JsonProperty("cover-medium")]
        public Image CoverMedium { get; set; }

        [JsonProperty("cover-large")]
        public Image CoverLarge { get; set; }

        [JsonProperty("icon")]
        public Image Icon { get; set; }

        [JsonProperty("foreground")]
        public object Foreground { get; set; }
    }
}