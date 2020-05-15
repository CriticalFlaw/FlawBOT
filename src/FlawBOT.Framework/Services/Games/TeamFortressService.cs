using FlawBOT.Framework.Models;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Models.GameEconomy;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TeamworkTF.Sharp;

namespace FlawBOT.Framework.Services
{
    public class TeamFortressService : HttpHandler
    {
        public static List<SchemaItem> ItemSchemaList { get; set; } = new List<SchemaItem>();
        public static SteamWebInterfaceFactory SteamInterface;

        #region SCHEMA

        public static SchemaItem GetSchemaItem(string query)
        {
            return ItemSchemaList.FirstOrDefault(n => n.ItemName.Contains(query, StringComparison.InvariantCultureIgnoreCase));
        }

        public static async Task<bool> UpdateTF2SchemaAsync()
        {
            try
            {
                SteamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
                var steam = SteamInterface.CreateSteamWebInterface<EconItems>(AppId.TeamFortress2, new HttpClient());
                var games = await steam.GetSchemaItemsForTF2Async().ConfigureAwait(false);
                ItemSchemaList.Clear();
                foreach (var game in games.Data.Result.Items)
                    if (!string.IsNullOrWhiteSpace(game.Name))
                        ItemSchemaList.Add(game);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating TF2 item schema. " + ex.Message);
                return false;
            }
        }

        #endregion SCHEMA

        #region MAPS

        public static async Task<List<MapName>> GetMapsBySearchAsync(string query)
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetMapsBySearchAsync(query).ConfigureAwait(false);
        }

        public static async Task<Map> GetMapStatsAsync(string query)
        {
            var map = GetMapsBySearchAsync(query).Result.FirstOrDefault().Name;
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetMapStatsAsync(map).ConfigureAwait(false);
        }

        public static async Task<MapThumbnail> GetMapThumbnailAsync(string query)
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetMapThumbnailAsync(query).ConfigureAwait(false);
        }

        #endregion MAPS

        #region NEWS

        public static async Task<List<News>> GetNewsOverviewAsync(int page = 0, string provider = "")
        {
            var results = new List<News>();
            if (page > 0)
                results = await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetNewsByPageAsync(page).ConfigureAwait(false);
            else if (provider != string.Empty)
                results = await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetNewsByProviderAsync(provider).ConfigureAwait(false);
            else
                results = await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetNewsOverviewAsync().ConfigureAwait(false);
            return results.Where(x => x.Type != "tf2-notification").ToList();
        }

        #endregion NEWS

        #region CREATORS

        public static async Task<List<Creator>> GetCreatorByIDAsync(ulong query)
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetCreatorByIDAsync(query.ToString()).ConfigureAwait(false);
        }

        #endregion CREATORS

        #region SERVERS

        public static async Task<GameMode> GetGameModeAsync(string query)
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetGameModeAsync(query).ConfigureAwait(false);
        }

        public static async Task<GameMode> GetGameModeListAsync()
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetGameModeListAsync().ConfigureAwait(false);
        }

        public static async Task<List<Server>> GetGameModeServerAsync(string query)
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetGameModeServerAsync(query).ConfigureAwait(false);
        }

        public static async Task<List<Server>> GetGameServerInfoAsync(string ip, int port)
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetGameServerInfoAsync(ip, port).ConfigureAwait(false);
        }

        public static async Task<List<ServerList>> GetCustomServerListsAsync()
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetCustomServerListsAsync().ConfigureAwait(false);
        }

        #endregion SERVERS
    }
}