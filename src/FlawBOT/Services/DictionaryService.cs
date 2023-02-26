using FlawBOT.Common;
using FlawBOT.Models.Dictionary;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class DictionaryService : HttpHandler
    {
        public static async Task<UrbanDictionaryDto> GetDictionaryDefinitionAsync(string query)
        {
            query = string.IsNullOrWhiteSpace(query) ? Resources.URL_Dictionary_Random : string.Format(Resources.URL_Dictionary, WebUtility.UrlEncode(query.Trim()));
            var response = await Http.GetStringAsync(query).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<UrbanDictionaryList>(response);
            if (result.ResultType == "no_results" || result.List.Count == 0) return null;
            return result.List[new Random().Next(result.List.Count)];
        }
    }
}