﻿using FlawBOT.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FlawBOT.Modules
{
    public static class RedditService
    {
        public static List<SyndicationItem> GetResults(string query, RedditCategory category)
        {
            try
            {
                query = string.Format(Resources.URL_Reddit, query.ToLowerInvariant(), category.GetPostCategory());
                using var reader = XmlReader.Create(query);
                return SyndicationFeed.Load(reader).Items?.ToList();
            }
            catch
            {
                return null;
            }
        }

        private static string GetPostCategory(this RedditCategory category)
        {
            return category switch
            {
                RedditCategory.Hot => "hot",
                RedditCategory.New => "new",
                RedditCategory.Top => "top",
                _ => throw new ArgumentException(Resources.ERR_REDDIT_UNKNOWN, nameof(category))
            };
        }
    }

    public enum RedditCategory
    {
        Hot,
        New,
        Top
    }
}