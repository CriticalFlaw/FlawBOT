using DSharpPlus.Entities;
using FlawBOT.Common;
using Genbox.Wikipedia;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class WikipediaService : HttpHandler
    {
        public static async Task<DiscordEmbed> GetWikipediaArticlesAsync(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                using var client = new WikipediaClient();
                var request = new WikiSearchRequest(query)
                {
                    Limit = 5
                };

                var results = await client.SearchAsync(request);
                if (results.QueryResult.SearchResults.Count < 5) return null;

                // TODO: Add pagination when supported for slash commands.
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Relevant articles found on Wikipedia.")
                    .WithColor(new DiscordColor("#6B6B6B"));

                foreach (var result in results.QueryResult.SearchResults.OrderBy(x => random.Next()).Take(5).ToList())
                {
                    var desc = Regex.Replace(result.Snippet.Length <= 300
                        ? string.IsNullOrEmpty(result.Snippet) ? "Article has not content." : result.Snippet
                        : result.Snippet[..150] + "...", "<[^>]*>", "");
                    output.AddField(result.Title, $"[[Link]({result.Url.AbsoluteUri})] {desc}");
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