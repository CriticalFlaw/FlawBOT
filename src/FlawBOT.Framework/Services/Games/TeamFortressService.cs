using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Models.GameEconomy;
using SteamWebAPI2.Utilities;
using TeamworkTF.Sharp;

namespace FlawBOT.Framework.Services
{
    public class TeamFortressService : HttpHandler
    {
        private static SteamWebInterfaceFactory _steamInterface;
        private static List<SchemaItem> ItemSchemaList { get; } = new List<SchemaItem>();

        #region NEWS

        public static async Task<List<News>> GetNewsArticlesAsync(int page = 0, string provider = "")
        {
            List<News> results;
            if (page > 0)
                results = await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetNewsByPageAsync(page)
                    .ConfigureAwait(false);
            else if (provider != string.Empty)
                results = await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetNewsByProviderAsync(provider)
                    .ConfigureAwait(false);
            else
                results = await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetNewsOverviewAsync()
                    .ConfigureAwait(false);
            return results.Where(x => x.Type != "tf2-notification").ToList();
        }

        #endregion NEWS

        #region CREATORS

        public static async Task<List<Creator>> GetContentCreatorAsync(ulong query)
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetYouTubeCreatorAsync(query.ToString())
                .ConfigureAwait(false);
        }

        #endregion CREATORS

        #region SCHEMA

        public static SchemaItem GetSchemaItem(string query)
        {
            return ItemSchemaList.FirstOrDefault(n =>
                n.ItemName.Contains(query, StringComparison.InvariantCultureIgnoreCase));
        }

        public static async Task<bool> UpdateTf2SchemaAsync()
        {
            try
            {
                _steamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
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

        public static async Task<GameMode> GetGameModeInfoAsync(string query)
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetGameModeAsync(query)
                .ConfigureAwait(false);
        }

        public static async Task<List<Server>> GetServersByGameModeAsync(string query)
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetServerListByGameModeAsync(query)
                .ConfigureAwait(false);
        }

        public static async Task<List<ServerList>> GetCustomServerListsAsync()
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetServerListsAsync()
                .ConfigureAwait(false);
        }

        public static string GetServerInfo(string address)
        {
            return new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetServerBanner(address);
        }

        #endregion SERVERS

        #region MAPS

        public static async Task<Map> GetMapStatsAsync(string query)
        {
            var map = new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetMapsBySearchAsync(query).Result
                .FirstOrDefault()?.Name;
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetMapStatsAsync(map)
                .ConfigureAwait(false);
        }

        public static async Task<MapThumbnail> GetMapThumbnailAsync(string query)
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetMapThumbnailAsync(query)
                .ConfigureAwait(false);
        }

        #endregion MAPS
    }
}