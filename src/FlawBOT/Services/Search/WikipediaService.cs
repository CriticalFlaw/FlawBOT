using WikipediaNet;
using WikipediaNet.Objects;

namespace FlawBOT.Services
{
    public class WikipediaService : HttpHandler
    {
        public static QueryResult GetWikipediaDataAsync(string query)
        {
            var wikipedia = new Wikipedia {Limit = 5};
            return wikipedia.Search(query);
        }
    }
}