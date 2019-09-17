using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using FlawBOT.Framework.Common;
using Reddit;
using Reddit.Controllers;

namespace FlawBOT.Framework.Services
{
    public static class RedditService
    {
        public static Subreddit GetSubredditAsync(string query)
        {
            var client = new RedditAPI(appId: TokenHandler.Tokens.RedditAppToken, refreshToken: TokenHandler.Tokens.RedditAccessToken, accessToken: TokenHandler.Tokens.RedditRefreshToken);
            var results = client.Subreddit(query).About();
            return results;
        }

        public static IReadOnlyList<SyndicationItem> GetSubredditPost(string query, RedditCategory category)
        {
            try
            {
                query = $"https://www.reddit.com{"/r/" + query.ToLowerInvariant()}/{GetPostCategory(category)}.rss";
                if (string.IsNullOrWhiteSpace(query))
                    throw new ArgumentException("Unknown reddit results", nameof(category));

                using (var reader = XmlReader.Create(query))
                {
                    var feed = SyndicationFeed.Load(reader);
                    return feed.Items?.Take(5).ToList().AsReadOnly();
                }
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