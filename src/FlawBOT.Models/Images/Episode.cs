using Newtonsoft.Json;

namespace FlawBOT.Models.Images
{
    public class Episode
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Key")]
        public string Key { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Director")]
        public string Director { get; set; }

        [JsonProperty("Writer")]
        public string Writer { get; set; }

        [JsonProperty("OriginalAirDate")]
        public string OriginalAirDate { get; set; }

        [JsonProperty("WikiLink")]
        public string WikiLink { get; set; }
    }
}