using Newtonsoft.Json;

namespace FlawBOT.Models.News
{
    public class NewsData
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("articles")]
        public List<Article> Articles { get; set; }
    }
}