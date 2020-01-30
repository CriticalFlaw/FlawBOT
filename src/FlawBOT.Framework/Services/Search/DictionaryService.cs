using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class DictionaryService : HttpHandler
    {
        public static async Task<DictionaryData> GetDictionaryDefinitionAsync(string query)
        {
            var results = await _http.GetStringAsync(Resources.API_Dictionary + "?term=" + WebUtility.UrlEncode(query.Trim())).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<DictionaryData>(results);
        }
    }
}