using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FlawBOT.Framework.Services
{
    public static class RedditService
    {
        public static List<SyndicationItem> GetResults(string query, RedditCategory category)
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