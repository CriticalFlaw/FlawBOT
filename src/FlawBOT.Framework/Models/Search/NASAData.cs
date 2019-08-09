using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    public class NASAData
    {
        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("explanation")]
        public string Description { get; set; }

        [JsonProperty("hdurl")]
        public string ImageHD { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string ImageSD { get; set; }
    }
}