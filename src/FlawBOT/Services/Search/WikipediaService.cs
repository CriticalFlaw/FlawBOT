using FlawBOT.Common;
using FlawBOT.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Services.Search
{
    public class WikipediaService : HttpHandler
    {
        private static readonly string base_url = "https://en.wikipedia.org//w/api.php?action=query&format=json&prop=info&redirects=1&formatversion=2&inprop=url&titles=";

        public static async Task<WikipediaData.WikipediaQuery.WikipediaPage> GetWikipediaDataAsync(string query)
        {
            var results = await _http.GetStringAsync(base_url + Uri.EscapeDataString(query));
            return JsonConvert.DeserializeObject<WikipediaData>(results).Query.Pages[0];
        }
    }
}