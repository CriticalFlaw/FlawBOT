using DSharpPlus.Entities;
using FlawBOT.Common;
using Genbox.Wikipedia;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class WikipediaService : HttpHandler
    {
        // Wikimedia requires a descriptive user agent, so reuse the shared client.
        // Kept for the lifetime of the app (never disposed) because it wraps the shared HttpClient.
        private static readonly WikipediaClient Client = new(Http);

        public static async Task<DiscordEmbed> GetWikipediaArticlesAsync(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                var client = Client;
                var request = new WikiSearchRequest(query.Trim())
                {
                    Limit = 5
                };
                var results = await client.SearchAsync(request).ConfigureAwait(false);
                var articles = results?.QueryResult?.SearchResults;
                if (articles is null || articles.Count == 0) return null;

                var output = new DiscordEmbedBuilder()
                    .WithTitle("Relevant articles found on Wikipedia.")
                    .WithColor(new DiscordColor("#6B6B6B"));
                foreach (var result in articles.Take(5))
                {
                    var desc = string.IsNullOrWhiteSpace(result.Snippet) ? "Article has no content." : result.Snippet.StripHtml().Truncate(300);
                    output.AddSafeField(result.Title, $"[[Link]({result.Url.AbsoluteUri})] {desc}");
                }

                return output.Build();
            }
            catch
            {
                return null;
            }
        }
    }
}
