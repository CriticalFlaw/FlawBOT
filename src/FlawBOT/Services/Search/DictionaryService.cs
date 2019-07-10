using FlawBOT.Common;
using FlawBOT.Models;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace FlawBOT.Services.Search
{
    public class DictionaryService : HttpHandler
    {
        public static async Task<DictionaryData> GetDictionaryForTermAsync(string query)
        {
            var results = await _http.GetStringAsync("http://api.urbandictionary.com/v0/define?term=" + WebUtility.UrlEncode(query.Trim()));
            return JsonConvert.DeserializeObject<DictionaryData>(results);
        }
    }
}