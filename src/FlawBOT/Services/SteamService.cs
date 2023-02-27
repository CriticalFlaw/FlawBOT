using FlawBOT.Common;
using FlawBOT.Properties;
using Steam.Models;
using Steam.Models.SteamCommunity;
using Steam.Models.SteamStore;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class SteamService : HttpHandler
    {
        private static SteamWebInterfaceFactory _steamInterface;

        private static ISteamWebResponse<IReadOnlyCollection<SteamAppModel>> SteamAppList { get; set; } = new SteamWebResponse<IReadOnlyCollection<SteamAppModel>>();

        /// <summary>
        /// Call the Steam API for data on a given game title.
        /// </summary>
        public static async Task<StoreAppDetailsDataModel> GetSteamAppAsync(string query)
        {
            try
            {
                var appId = SteamAppList.Data.First(n => string.Equals(n.Name, query, StringComparison.InvariantCultureIgnoreCase)).AppId;
                return await new SteamWebInterfaceFactory(Program.Settings.Tokens.SteamToken).CreateSteamStoreInterface().GetStoreAppDetailsAsync(appId).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<bool> UpdateSteamAppListAsync(string token)
        {
            try
            {
                _steamInterface = new SteamWebInterfaceFactory(token);
                SteamAppList = await _steamInterface.CreateSteamWebInterface<SteamApps>(new HttpClient()).GetAppListAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.ERR_STEAM_LIST, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Call the Steam API for summary data on a given user.
        /// </summary>
        public static async Task<ISteamWebResponse<PlayerSummaryModel>> GetSteamProfileAsync(string token, string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                _steamInterface = new SteamWebInterfaceFactory(token);
                var steam = _steamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
                var userId = ulong.TryParse(query, out var steamId) ? steamId : 0;
                if (userId != 0) return await steam.GetPlayerSummaryAsync(userId).ConfigureAwait(false);
                var data = GetSteamUserId(query).Result;
                if (data is null) return null;
                return await steam.GetPlayerSummaryAsync(data.Data).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Call the Steam API for the id of a given user.
        /// </summary>
        public static async Task<ISteamWebResponse<ulong>> GetSteamUserId(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                var steam = _steamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
                return await steam.ResolveVanityUrlAsync(query.Replace(" ", "")).ConfigureAwait(false) ?? null;
            }
            catch
            {
                return null;
            }
        }
    }
}