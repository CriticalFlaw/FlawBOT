using FlawBOT.Common;
using FlawBOT.Models;
using Newtonsoft.Json;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Services.Search
{
    public class SteamService : HttpHandler
    {
        public static uint GetSteamAppAsync(string query)
        {
            var random = new Random();
            var game = SharedData.SteamAppList.FirstOrDefault(n => n.Value.ToUpperInvariant() == query.ToUpperInvariant()).Key;
            var results = (game >= 0) ? game : SharedData.SteamAppList.Keys.ToArray()[random.Next(0, SharedData.SteamAppList.Keys.Count - 1)];
            return results;
        }

        public static async Task<SteamData> GetSteamAppsListAsync()
        {
            var result = await _http.GetStringAsync("http://api.steampowered.com/ISteamApps/GetAppList/v0002/");
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

        public static async Task<bool> UpdateSteamListAsync()
        {
            try
            {
                var client = new SteamApps(SharedData.Tokens.SteamToken);
                var games = (await client.GetAppListAsync()).Data;
                SharedData.SteamAppList.Clear();
                foreach (var game in games)
                    if (!string.IsNullOrWhiteSpace(game.Name))
                        SharedData.SteamAppList.Add(Convert.ToUInt32(game.AppId), game.Name);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating Steam games list. " + ex.Message);
                return false;
            }
        }
    }
}