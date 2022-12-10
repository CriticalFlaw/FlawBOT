using Genbox.Wikipedia;
using System.Threading.Tasks;

namespace FlawBOT.Services.Lookup
{
    public class WikipediaService : HttpHandler
    {
        public static Task<WikiSearchResponse> GetWikipediaDataAsync(string query)
        {
            using var client = new WikipediaClient();
            return client.SearchAsync(query);
        }
    }
}