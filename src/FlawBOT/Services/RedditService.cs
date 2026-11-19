using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace FlawBOT.Services
{
    public partial class RedditService : HttpHandler
    {
        public static async Task<DiscordEmbed> GetRedditResultsAsync(string query, RedditCategory redditCategory)
        {
            try
            {
                var subreddit = NormalizeSubreddit(query);
                if (subreddit is null) return null;

                // Reddit rejects requests without a user agent, so fetch the feed with the shared client.
                var url = string.Format(Resources.URL_Reddit, subreddit, GetCategoryPath(redditCategory));
                await using var stream = await Http.GetStreamAsync(url).ConfigureAwait(false);
                using var reader = XmlReader.Create(stream, new XmlReaderSettings { Async = true, DtdProcessing = DtdProcessing.Ignore });
                return BuildFeedEmbed(SyndicationFeed.Load(reader), subreddit);
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed BuildFeedEmbed(SyndicationFeed feed, string subreddit)
        {
            var results = feed?.Items?.Where(x => x.Links.Count > 0).OrderBy(_ => Random.Next()).Take(5).ToList();
            if (results is null || results.Count == 0) return null;

            var output = new DiscordEmbedBuilder()
                .WithFooter($"Search results for r/{subreddit} on Reddit")
                .WithColor(new DiscordColor("#FF4500"));
            foreach (var x in results)
                output.AddSafeField(x.Authors.FirstOrDefault()?.Name ?? "Unknown author", $"[{x.Title.Text.Truncate(500)}]({x.Links.First().Uri})");
            return output.Build();
        }

        /// <summary>
        ///     Accepts "aww", "r/aww" or "/r/aww" and rejects anything that isn't a valid subreddit name.
        /// </summary>
        public static string NormalizeSubreddit(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;
            var name = query.Trim().Trim('/');
            if (name.StartsWith("r/", StringComparison.OrdinalIgnoreCase)) name = name[2..];
            return SubredditRegex().IsMatch(name) ? name.ToLowerInvariant() : null;
        }

        public static string GetCategoryPath(RedditCategory category)
        {
            return category switch
            {
                RedditCategory.Hot => "hot",
                RedditCategory.New => "new",
                _ => "top"
            };
        }

        [GeneratedRegex("^[A-Za-z0-9_]{2,21}$")]
        private static partial Regex SubredditRegex();
    }

    public enum RedditCategory
    {
        Hot,
        New,
        Top
    }
}
