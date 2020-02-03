using FlawBOT.Framework.Models;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Models.GameEconomy;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TeamworkAPI;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class TeamFortressService : HttpHandler
    {
        public static List<SchemaItem> ItemSchemaList { get; set; } = new List<SchemaItem>();
        public static SteamWebInterfaceFactory SteamInterface;

        public static SchemaItem GetSchemaItem(string query)
        {
            return ItemSchemaList.FirstOrDefault(n => n.ItemName.Contains(query, StringComparison.InvariantCultureIgnoreCase));
        }

        public static async Task<bool> UpdateTF2SchemaAsync()
        {
            try
            {
                SteamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
                var steam = SteamInterface.CreateSteamWebInterface<EconItems>(new HttpClient(), EconItemsAppId.TeamFortress2);
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

        public static async Task<List<News>> GetNewsOverviewAsync()
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetNewsOverviewAsync().ConfigureAwait(false);
        }

        public static async Task<List<Server>> GetServersAsync(string query)
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetGameModeServerAsync(query).ConfigureAwait(false);
        }

        public static async Task<Map> GetMapStatsAsync(string query)
        {
            return await new TeamworkClient(TokenHandler.Tokens.TeamworkToken).GetMapStatsAsync(query).ConfigureAwait(false);
        }
    }
}