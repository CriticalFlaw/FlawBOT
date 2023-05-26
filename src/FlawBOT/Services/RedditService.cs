using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FlawBOT.Services
{
    public class RedditService : HttpHandler
    {
        public static DiscordEmbed GetRedditResults(string query, RedditCategory redditCategory)
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
                var results = result.OrderBy(x => random.Next()).Take(5).ToList();

                // TODO: Add pagination when supported for slash commands.
                var output = new DiscordEmbedBuilder()
                    .WithFooter($"Search results for {query} on Reddit")
                    .WithColor(new DiscordColor("#FF4500"));

                foreach (var x in results)
                    output.AddField(x.Authors.FirstOrDefault()?.Name, $"[{(x.Title.Text.Length < 500 ? x.Title.Text : x.Title.Text.Take(500) + "...")}]({x.Links.First().Uri})");
                return output.Build();
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