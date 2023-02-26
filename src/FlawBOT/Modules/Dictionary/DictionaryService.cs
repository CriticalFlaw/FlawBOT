using FlawBOT.Common;
using FlawBOT.Models.Dictionary;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Dictionary
{
    public class DictionaryService : HttpHandler
    {
        public static async Task<List<UrbanDictionaryDto>> GetDictionaryDefinitionAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;
            var response = await Http.GetStringAsync(string.Format(Resources.URL_Dictionary, WebUtility.UrlEncode(query.Trim()))).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<UrbanDictionaryList>(response);
            return result.ResultType == "no_results" || result.List.Count == 0 ? null : result.List;
        }
    }
}