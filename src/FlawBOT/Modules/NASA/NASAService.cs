using FlawBOT.Common;
using FlawBOT.Models.NASA;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FlawBOT.Modules.NASA
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