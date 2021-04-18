using Genbox.Wikipedia;
using Genbox.Wikipedia.Objects;

namespace FlawBOT.Services
{
    public class WikipediaService : HttpHandler
    {
        public static QueryResult GetWikipediaDataAsync(string query)
        {
            var wikipedia = new WikipediaClient();
            return wikipedia.Search(query);
        }
    }
}