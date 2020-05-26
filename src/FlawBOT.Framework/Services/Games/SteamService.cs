using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Microsoft.Extensions.Options;
using Steam.Models.SteamCommunity;
using Steam.Models.SteamStore;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace FlawBOT.Framework.Services
{
    public class SteamService : HttpHandler
    {
        public static SteamWebInterfaceFactory SteamInterface;
        public static Dictionary<uint, string> SteamAppList { get; set; } = new Dictionary<uint, string>();

        /// <remarks>https://github.com/babelshift/SteamWebAPI2/issues/81</remarks>
        public static async Task<StoreAppDetailsDataModel> GetSteamAppAsync(string query)
        {
            try
            {
                SteamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
                var factoryOptions = new SteamWebInterfaceFactoryOptions
                {
                    SteamWebApiKey = TokenHandler.Tokens.SteamToken
                };
                var store = new SteamWebInterfaceFactory(Options.Create(factoryOptions)).CreateSteamStoreInterface();
                var list = await SteamInterface.CreateSteamWebInterface<SteamApps>(new HttpClient()).GetAppListAsync();
                var appId = list.Data.First(n => string.Equals(n.Name, query, StringComparison.InvariantCultureIgnoreCase)).AppId;
                return await store.GetStoreAppDetailsAsync(appId).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<SteamCommunityProfileModel> GetSteamProfileAsync(string query)
        {
            try
            {
                SteamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
                var steam = SteamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
                if (ulong.TryParse(query, out var steamId))
                    return await steam.GetCommunityProfileAsync(steamId).ConfigureAwait(false);
                return await steam.GetCommunityProfileAsync(GetSteamUserID(query).Result.Data).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<ISteamWebResponse<PlayerSummaryModel>> GetSteamSummaryAsync(string query)
        {
            try
            {
                SteamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
                var steam = SteamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
                if (ulong.TryParse(query, out var steamId))
                    return await steam.GetPlayerSummaryAsync(steamId).ConfigureAwait(false);
                return await steam.GetPlayerSummaryAsync(GetSteamUserID(query).Result.Data).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<bool> UpdateSteamListAsync()
        {
            try
            {
                SteamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
                var steam = SteamInterface.CreateSteamWebInterface<SteamApps>(new HttpClient());
                var games = (await steam.GetAppListAsync().ConfigureAwait(false)).Data;
                SteamAppList.Clear();
                foreach (var game in games)
                    if (!string.IsNullOrWhiteSpace(game.Name))
                        SteamAppList.Add(Convert.ToUInt32(game.AppId), game.Name);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.ERR_STEAM_LIST, ex.Message);
                return false;
            }
        }

        public static async Task<ISteamWebResponse<ulong>> GetSteamUserID(string query)
        {
            try
            {
                var steam = SteamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
                return await steam.ResolveVanityUrlAsync(query).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }
    }
}