using FlawBOT.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services.Search
{
    public class DictionaryService
    {
        private static readonly string base_url = "http://api.urbandictionary.com/v0/define?term=";
        private static readonly HttpClient http = new HttpClient();

        public static async Task<DictionaryData> GetDictionaryForTermAsync(string query)
        {
            var data = await http.GetStringAsync(base_url + WebUtility.UrlEncode(query.Trim()));
            return JsonConvert.DeserializeObject<DictionaryData>(data);
        }
    }
}