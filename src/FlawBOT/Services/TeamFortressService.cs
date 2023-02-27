using FlawBOT.Common;
using FlawBOT.Properties;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Models.GameEconomy;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TeamworkTF.Sharp;

namespace FlawBOT.Services
{
    public class TeamFortressService : HttpHandler
    {
        private static SteamWebInterfaceFactory _steamInterface;
        private static List<SchemaItem> ItemSchemaList { get; } = new();

        #region NEWS

        public static async Task<List<News>> GetNewsArticlesAsync(string token, int page = 0, string provider = "")
        {
            List<News> results;
            if (page > 0)
                results = await new TeamworkAPI(token).GetNewsByPageAsync(page).ConfigureAwait(false);
            else if (provider != string.Empty)
                results = await new TeamworkAPI(token).GetNewsByProviderAsync(provider).ConfigureAwait(false);
            else
                results = await new TeamworkAPI(token).GetNewsOverviewAsync().ConfigureAwait(false);
            return results.Where(x => x.Type != "tf2-notification").ToList();
        }

        #endregion NEWS

        #region CREATORS

        public static async Task<List<User>> GetContentCreatorAsync(string token, ulong query)
        {
            return await new TeamworkAPI(token)
                .GetCommunityUserAsync(query.ToString())
                .ConfigureAwait(false);
        }

        #endregion CREATORS

        #region SCHEMA

        public static SchemaItem GetSchemaItem(string query)
        {
            return ItemSchemaList.Find(n => n.ItemName.Contains(query, StringComparison.InvariantCultureIgnoreCase));
        }

        public static async Task<bool> UpdateTF2SchemaAsync(string token)
        {
            try
            {
                _steamInterface = new SteamWebInterfaceFactory(token);
                var steam = _steamInterface.CreateSteamWebInterface<EconItems>(AppId.TeamFortress2, new HttpClient());
                var games = await steam.GetSchemaItemsForTF2Async().ConfigureAwait(false);
                ItemSchemaList.Clear();
                foreach (var game in games.Data.Result.Items)
                    if (!string.IsNullOrWhiteSpace(game.Name))
                        ItemSchemaList.Add(game);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.ERR_TF2_LIST, ex.Message);
                return false;
            }
        }

        #endregion SCHEMA

        #region SERVERS

        public static async Task<GameMode> GetGameModeInfoAsync(string token, string query)
        {
            return await new TeamworkAPI(token).GetGameModeAsync(query).ConfigureAwait(false);
        }

        public static async Task<List<Server>> GetServersByGameModeAsync(string token, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;
            return await new TeamworkAPI(token).GetServerListByGameModeAsync(query).ConfigureAwait(false);
        }

        public static async Task<List<ServerList>> GetCustomServerListsAsync(string token)
        {
            return await new TeamworkAPI(token).GetServerListsAsync().ConfigureAwait(false);
        }

        public static string GetServerInfo(string token, string address)
        {
            return new TeamworkAPI(token).GetServerBanner(address);
        }

        #endregion SERVERS

        #region MAPS

        public static async Task<Map> GetMapStatsAsync(string token, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;
            var map = new TeamworkAPI(token).GetMapsBySearchAsync(query).Result.FirstOrDefault()?.Name;
            return await new TeamworkAPI(token).GetMapStatsAsync(map).ConfigureAwait(false);
        }

        public static async Task<MapThumbnail> GetMapThumbnailAsync(string token, string query)
        {
            return await new TeamworkAPI(token).GetMapThumbnailAsync(query).ConfigureAwait(false);
        }

        #endregion MAPS
    }
}