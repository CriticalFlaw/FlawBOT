using System.Threading.Tasks;
using FlawBOT.Models;
using FlawBOT.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Services.Lookup
{
    public class NasaService : HttpHandler
    {
        public static async Task<NasaData> GetNasaImageAsync(string token)
        {
            var results = await Http
                .GetStringAsync(string.Format(Resources.URL_NASA, token))
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<NasaData>(results);
        }
    }
}