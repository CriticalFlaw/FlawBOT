﻿using Newtonsoft.Json;

namespace FlawBOT.Models.News
{
    public class Article
    {
        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("urlToImage")]
        public string UrlImage { get; set; }

        [JsonProperty("publishedAt")]
        public DateTime PublishDate { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}