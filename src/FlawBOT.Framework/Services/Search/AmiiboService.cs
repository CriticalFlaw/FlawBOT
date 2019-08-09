using System.Threading.Tasks;
using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Services
{
    public class AmiiboService : HttpHandler
    {
        public static async Task<AmiiboData> GetAmiiboFigurineAsync(string query)
        {
            try
            {
                var results = await _http.GetStringAsync(Resources.API_Amiibo + "?name=" + query.ToLowerInvariant());
                return JsonConvert.DeserializeObject<AmiiboData>(results);
            }
            catch
            {
                return null;
            }
        }
    }
}