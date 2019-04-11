using Newtonsoft.Json;

namespace FlawBOT.Models
{
    public class WikipediaData
    {
        [JsonProperty("Query")]
        public WikipediaQuery Query { get; set; }

        public class WikipediaQuery
        {
            [JsonProperty("Pages")]
            public WikipediaPage[] Pages { get; set; }

            public class WikipediaPage
            {
                [JsonProperty("Missing")]
                public bool Missing { get; set; }

                [JsonProperty("Title")]
                public string Title { get; set; }

                [JsonProperty("FullUrl")]
                public string FullUrl { get; set; }
            }
        }
    }
}