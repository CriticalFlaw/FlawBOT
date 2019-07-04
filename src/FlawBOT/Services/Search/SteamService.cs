using FlawBOT.Common;
using FlawBOT.Models;
using Newtonsoft.Json;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
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

        public static async Task<SteamCommunityProfileModel> GetSteamUserProfileAsync(string query)
        {
            var steam = new SteamUser(SharedData.Tokens.SteamToken);
            SteamCommunityProfileModel profile;
            try
            {
                var decode = await steam.ResolveVanityUrlAsync(query);
                profile = await steam.GetCommunityProfileAsync(decode.Data).ConfigureAwait(false);
            }
            catch
            {
                profile = await steam.GetCommunityProfileAsync(ulong.Parse(query)).ConfigureAwait(false);
            }
            return profile;
        }

        public static async Task<ISteamWebResponse<PlayerSummaryModel>> GetSteamUserSummaryAsync(string query)
        {
            var steam = new SteamUser(SharedData.Tokens.SteamToken);
            ISteamWebResponse<PlayerSummaryModel> summary;
            try
            {
                var decode = await steam.ResolveVanityUrlAsync(query);
                summary = await steam.GetPlayerSummaryAsync(decode.Data).ConfigureAwait(false);
            }
            catch
            {
                summary = await steam.GetPlayerSummaryAsync(ulong.Parse(query)).ConfigureAwait(false);
            }
            return summary;
        }
    }
}