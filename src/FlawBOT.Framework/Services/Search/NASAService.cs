using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class NASAService : HttpHandler
    {
        public static async Task<NASAData> GetNASAImage()
        {
            var results = await _http.GetStringAsync(Resources.API_NASA + "?api_key=" + TokenHandler.Tokens.NASAToken).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<NASAData>(results);
        }
    }
}