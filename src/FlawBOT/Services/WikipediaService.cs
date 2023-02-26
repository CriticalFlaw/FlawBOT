using FlawBOT.Common;
using Genbox.Wikipedia;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class WikipediaService : HttpHandler
    {
        public static async Task<WikiSearchResponse> GetWikipediaDataAsync(string query)
        {
            using var client = new WikipediaClient();
            return await client.SearchAsync(query);
        }
    }
}