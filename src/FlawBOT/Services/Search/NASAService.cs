using System.Threading.Tasks;
using FlawBOT.Common;
using FlawBOT.Models;
using FlawBOT.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Services
{
    public class NasaService : HttpHandler
    {
        public static async Task<NasaData> GetNasaImageAsync()
        {
            var results = await Http.GetStringAsync(string.Format(Resources.URL_NASA, Program.Settings.Tokens.NasaToken))
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<NasaData>(results);
        }
    }
}