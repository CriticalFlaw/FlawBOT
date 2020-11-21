using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlawBOT.Models
{
    public class NewsData
    {
        [JsonProperty("status")] public string Status { get; set; }

        [JsonProperty("articles")] public List<Article> Articles { get; set; }
    }

    public class Article
    {
        [JsonProperty("author")] public string Author { get; set; }

        [JsonProperty("title")] public string Title { get; set; }

        [JsonProperty("description")] public string Description { get; set; }

        [JsonProperty("url")] public string Url { get; set; }

        [JsonProperty("urlToImage")] public string UrlImage { get; set; }

        [JsonProperty("publishedAt")] public DateTime PublishDate { get; set; }

        [JsonProperty("content")] public string Content { get; set; }
    }
}