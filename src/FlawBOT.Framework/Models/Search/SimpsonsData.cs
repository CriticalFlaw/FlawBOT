using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    public class SimpsonsData
    {
        [JsonProperty("Episode")]
        public Episode Episode { get; set; }

        [JsonProperty("Frame")]
        public Frame Frame { get; set; }
    }

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

    public class Frame
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Episode")]
        public string Episode { get; set; }

        [JsonProperty("Timestamp")]
        public int Timestamp { get; set; }
    }
}