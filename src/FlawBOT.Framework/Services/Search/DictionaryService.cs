using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
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