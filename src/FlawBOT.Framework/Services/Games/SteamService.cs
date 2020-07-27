using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using Microsoft.Extensions.Options;
using Steam.Models.SteamCommunity;
using Steam.Models.SteamStore;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace FlawBOT.Framework.Services
{
    public class SteamService : HttpHandler
    {
        private static SteamWebInterfaceFactory _steamInterface;

        /// <remarks>https://github.com/babelshift/SteamWebAPI2/issues/81</remarks>
        public static async Task<StoreAppDetailsDataModel> GetSteamAppAsync(string query)
        {
            try
            {
                _steamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
                var factoryOptions = new SteamWebInterfaceFactoryOptions
                {
                    SteamWebApiKey = TokenHandler.Tokens.SteamToken
                };
                var store = new SteamWebInterfaceFactory(Options.Create(factoryOptions)).CreateSteamStoreInterface();
                var list = await _steamInterface.CreateSteamWebInterface<SteamApps>(new HttpClient()).GetAppListAsync();
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
                _steamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
                var steam = _steamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
                if (ulong.TryParse(query, out var steamId))
                    return await steam.GetCommunityProfileAsync(steamId).ConfigureAwait(false);
                return await steam.GetCommunityProfileAsync(GetSteamUserId(query).Result.Data).ConfigureAwait(false);
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
                _steamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
                var steam = _steamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
                if (ulong.TryParse(query, out var steamId))
                    return await steam.GetPlayerSummaryAsync(steamId).ConfigureAwait(false);
                return await steam.GetPlayerSummaryAsync(GetSteamUserId(query).Result.Data).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<ISteamWebResponse<ulong>> GetSteamUserId(string query)
        {
            try
            {
                var steam = _steamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
                return await steam.ResolveVanityUrlAsync(query).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }
    }
}