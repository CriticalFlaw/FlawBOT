using FlawBOT.Common;
using FlawBOT.Properties;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FlawBOT.Services
{
    public class RedditService : HttpHandler
    {
        public static List<SyndicationItem> GetResults(string query, RedditCategory redditCategory)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                var category = redditCategory switch
                {
                    RedditCategory.Hot => "hot",
                    RedditCategory.New => "new",
                    _ => "top",
                };

                query = string.Format(Resources.URL_Reddit, query.ToLowerInvariant(), category);
                using var reader = XmlReader.Create(query);
                var result = SyndicationFeed.Load(reader).Items?.ToList();
                if (result.Count < 5) return null;
                return result.OrderBy(x => random.Next()).Take(5).ToList();
            }
            catch
            {
                return null;
            }
        }
    }

    public enum RedditCategory
    {
        Hot,
        New,
        Top
    }
}