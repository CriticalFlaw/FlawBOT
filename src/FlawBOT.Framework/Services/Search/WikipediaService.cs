using FlawBOT.Framework.Models;
using WikipediaNet;
using WikipediaNet.Objects;

namespace FlawBOT.Framework.Services
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