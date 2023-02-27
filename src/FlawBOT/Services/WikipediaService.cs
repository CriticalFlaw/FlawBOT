using FlawBOT.Common;
using Genbox.Wikipedia;
using Genbox.Wikipedia.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class WikipediaService : HttpHandler
    {
        public static async Task<List<SearchResult>> GetWikipediaDataAsync(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                using var client = new WikipediaClient();
                var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                var result = await client.SearchAsync(query);
                if (result.QueryResult.SearchResults.Count < 5) return null;
                return result.QueryResult.SearchResults.OrderBy(x => random.Next()).Take(5).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}