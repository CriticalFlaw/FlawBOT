using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;
using Steam.Models.SteamCommunity;
using Steam.Models.SteamStore;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace FlawBOT.Framework.Services
{
    public class SteamService : HttpHandler
    {
        public static Dictionary<uint, string> SteamAppList { get; set; } = new Dictionary<uint, string>();

        public static async Task<StoreAppDetailsDataModel> GetSteamAppAsync(string query)
        {
            var store = new SteamStore();
            var random = new Random();
            var game = SteamAppList.FirstOrDefault(n => n.Value.ToUpperInvariant() == query.ToUpperInvariant()).Key;
            var appId = (game >= 0) ? game : SteamAppList.Keys.ToArray()[random.Next(0, SteamAppList.Keys.Count - 1)];
            return await store.GetStoreAppDetailsAsync(appId).ConfigureAwait(false);
        }

        public static async Task<SteamData> GetSteamAppsListAsync()
        {
            var result = await _http.GetStringAsync(Resources.API_SteamGames);
            return JsonConvert.DeserializeObject<SteamData>(result);
        }

        public static async Task<SteamCommunityProfileModel> GetSteamUserProfileAsync(string query)
        {
            var steam = new SteamUser(TokenHandler.Tokens.SteamToken);
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
            var steam = new SteamUser(TokenHandler.Tokens.SteamToken);
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
                var client = new SteamApps(TokenHandler.Tokens.SteamToken);
                var games = (await client.GetAppListAsync()).Data;
                SteamAppList.Clear();
                foreach (var game in games)
                    if (!string.IsNullOrWhiteSpace(game.Name))
                        SteamAppList.Add(Convert.ToUInt32(game.AppId), game.Name);
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