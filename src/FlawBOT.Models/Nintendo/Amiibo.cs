using Newtonsoft.Json;

namespace FlawBOT.Models.Nintendo
{
    public class Amiibo
    {
        [JsonProperty("amiiboSeries")]
        public string AmiiboSeries { get; set; }

        [JsonProperty("gameSeries")]
        public string GameSeries { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("release")]
        public Release ReleaseDate { get; set; }
    }
}