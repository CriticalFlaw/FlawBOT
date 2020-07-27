using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Services
{
    public class NasaService : HttpHandler
    {
        public static async Task<NasaData> GetNasaImageAsync()
        {
            var results = await Http.GetStringAsync(Resources.API_NASA + "?api_key=" + TokenHandler.Tokens.NasaToken)
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<NasaData>(results);
        }
    }
}