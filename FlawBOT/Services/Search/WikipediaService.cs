using FlawBOT.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services.Search
{
    public class WikipediaService
    {
        private static readonly string base_url = "https://en.wikipedia.org//w/api.php?action=query&format=json&prop=info&redirects=1&formatversion=2&inprop=url&titles=";
        private static readonly HttpClient http = new HttpClient();

        public static async Task<WikipediaData> GetWikipediaDataAsync(string query)
        {
            var data = await http.GetStringAsync(base_url + Uri.EscapeDataString(query));
            return JsonConvert.DeserializeObject<WikipediaData>(data);
        }
    }
}