using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class WikipediaService : HttpHandler
    {
        public static async Task<WikipediaData.WikipediaQuery.WikipediaPage> GetWikipediaDataAsync(string query)
        {
            var results = await _http.GetStringAsync("https://en.wikipedia.org//w/api.php?action=query&format=json&prop=info&redirects=1&formatversion=2&inprop=url&titles=" + Uri.EscapeDataString(query));
            return JsonConvert.DeserializeObject<WikipediaData>(results).Query.Pages[0];
        }
    }
}