using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Services
{
    public class AmiiboService : HttpHandler
    {
        public static async Task<AmiiboData> GetAmiiboDataAsync(string query)
        {
            try
            {
                var results = await Http.GetStringAsync(string.Format(Resources.API_Amiibo, query.ToLowerInvariant()))
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