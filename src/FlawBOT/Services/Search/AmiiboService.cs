using System.Threading.Tasks;
using FlawBOT.Models;
using FlawBOT.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Services
{
    public class AmiiboService : HttpHandler
    {
        public static async Task<AmiiboData> GetAmiiboDataAsync(string query)
        {
            try
            {
                var results = await Http.GetStringAsync(string.Format(Resources.URL_Amiibo, query.ToLowerInvariant()))
                    .ConfigureAwait(false);
                return JsonConvert.DeserializeObject<AmiiboData>(results);
            }
            catch
            {
                return null;
            }
        }
    }
}