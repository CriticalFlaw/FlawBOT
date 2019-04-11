using FlawBOT.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services.Search
{
    public class SteamService
    {
        private static readonly string base_url = "http://api.steampowered.com/ISteamApps/GetAppList/v0002/";
        private static readonly HttpClient http = new HttpClient();

        public static async Task<SteamData> GetSteamAppsListAsync()
        {
            var result = await http.GetStringAsync(base_url);
            return JsonConvert.DeserializeObject<SteamData>(result);
        }
    }
}