using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FlawBOT.Framework.Services
{
    public static class RedditService
    {
        public static DiscordEmbed GetEmbeddedResults(string query, RedditCategory category = RedditCategory.Hot)
        {
            var results = GetResults(query, category);
            if (results is null || results.Count == 0)
                return new DiscordEmbedBuilder
                {
                    Description = ":warning: No results found!",
                    Color = new DiscordColor("#FF4500")
                };
            results = (results.Count > 5) ? results.Take(5).ToList() : results;
            var output = new DiscordEmbedBuilder { Color = new DiscordColor("#FF4500") };
            foreach (var result in results)
                output.AddField(result.Title.Text.Length < 500 ? result.Title.Text : result.Title.Text.Take(500) + "...", result.Links.First().Uri.ToString());
            return output.Build();
        }

        private static List<SyndicationItem> GetResults(string query, RedditCategory category)
        {
            try
            {
                query = $"https://www.reddit.com/r/{query.ToLowerInvariant()}/{GetPostCategory(category)}.rss";
                using var reader = XmlReader.Create(query);
                return SyndicationFeed.Load(reader).Items?.ToList();
            }
            catch
            {
                return null;
            }
        }

        public static string GetPostCategory(this RedditCategory category)
        {
            switch (category)
            {
                case RedditCategory.Hot: return "hot";
                case RedditCategory.New: return "new";
                case RedditCategory.Top: return "top";
            }
            throw new ArgumentException("Unknown reddit category", nameof(category));
        }
    }

    public enum RedditCategory
    {
        Hot,
        New,
        Top
    }
}