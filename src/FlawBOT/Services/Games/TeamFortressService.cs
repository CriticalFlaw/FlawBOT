using BackpackTfApi;
using BackpackTfApi.Economy.PriceHistory.Models;
using BackpackTfApi.Economy.Prices.Models;
using BackpackTfApi.Economy.SpecialItems;
using BackpackTfApi.SteamUser.UserInventory.Models;
using FlawBOT.Common;
using FlawBOT.Models;
using Newtonsoft.Json;
using Steam.Models.TF2;
using SteamWebAPI2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Services.Games
{
    public class TeamFortressService : HttpHandler
    {
        private static readonly string news_url = "https://teamwork.tf/api/v1/news";
        private static readonly string servers_url = "https://teamwork.tf/api/v1/quickplay";
        private static readonly string maps_url = "https://teamwork.tf/api/v1/map-stats/map";

        public static SchemaItemModel GetSchemaItemAsync(string query)
        {
            var results = SharedData.TF2ItemSchema.FirstOrDefault(n => n.Value.Name.ToUpperInvariant() == query.ToUpperInvariant()).Value;
            return results;
        }

        #region TEAMWORK.TF

        public static async Task<List<TeamworkNews>> GetNewsOverviewAsync()
        {
            var results = await _http.GetStringAsync(news_url + $"?key={SharedData.Tokens.TeamworkToken}");
            return JsonConvert.DeserializeObject<List<TeamworkNews>>(results);
        }

        public static async Task<List<TeamworkServer>> GetServersAsync(string query)
        {
            var results = await _http.GetStringAsync(servers_url + $"/{query}/servers?key={SharedData.Tokens.TeamworkToken}");
            return JsonConvert.DeserializeObject<List<TeamworkServer>>(results);
        }

        public static async Task<TeamworkMap> GetMapStatsAsync(string query)
        {
            var results = await _http.GetStringAsync(maps_url + $"/{query}?key={SharedData.Tokens.TeamworkToken}");
            return JsonConvert.DeserializeObject<TeamworkMap>(results);
        }

        public static string NormalizedGameMode(string input)
        {
            switch (input.ToUpperInvariant())
            {
                default:
                case "AD":
                case "ATTACK-DEFENSE":
                    return "attack-defend";

                case "CAPTURE-THE-FLAG":
                    return "ctf";

                case "CP":
                    return "control-point";

                case "KING OF THE HILL":
                    return "koth";

                case "MANN VS MACHINE":
                    return "mvm";

                case "PL":
                    return "payload";

                case "PLR":
                    return "payload-race";
            }
        }

        #endregion TEAMWORK.TF

        #region BACKPACK.TF

        /// <summary>
        /// Returns price history for the specified item.
        /// </summary>
        /// <param name="itemName"></param>
        public static PriceHistoryData GetPriceHistory(string itemName)
        {
            var bpUser = new BackpackTfUser(SharedData.Tokens.SteamID, SharedData.Tokens.BackpackToken, SharedData.Tokens.BackpackAccess);
            var results = bpUser.GetPriceHistory(itemName, 1);
            return results;
        }

        /// <summary>
        /// Fetches item prices for the specified API key. A request may be sent once every 60 seconds.
        /// </summary>
        public static PricesData GetItemPrices()
        {
            var bpUser = new BackpackTfUser(SharedData.Tokens.SteamID, SharedData.Tokens.BackpackToken, SharedData.Tokens.BackpackAccess);
            var results = bpUser.GetItemPrices();
            return results;
        }

        /// <summary>
        /// Gets special items for the specified API key.
        /// </summary>
        public static SpecialItemsData GetSpecialItems()
        {
            var bpUser = new BackpackTfUser(SharedData.Tokens.SteamID, SharedData.Tokens.BackpackToken, SharedData.Tokens.BackpackAccess);
            var results = bpUser.GetSpecialItems();
            return results;
        }

        /// <summary>
        /// Fetches all currently open classifieds that are on backpack.tf
        /// </summary>
        public static BackpackTfApi.UserToken.Classifieds.ClassifiedsSearch.Models.Response GetClassifieds()
        {
            var bpUser = new BackpackTfUser(SharedData.Tokens.SteamID, SharedData.Tokens.BackpackToken, SharedData.Tokens.BackpackAccess);
            var results = bpUser.GetClassifieds();
            return results;
        }

        /// <summary>
        /// Fetches the currently opened user's classifieds from backpack.tf
        /// </summary>
        public static BackpackTfApi.UserToken.Classifieds.UserListings.Models.Response GetOwnClassifieds()
        {
            var bpUser = new BackpackTfUser(SharedData.Tokens.SteamID, SharedData.Tokens.BackpackToken, SharedData.Tokens.BackpackAccess);
            var results = bpUser.GetOwnClassifieds();
            return results;
        }

        /// <summary>
        /// Fetches a user's inventory.
        /// </summary>
        /// <param name="steamID"></param>
        public static BackpackTfApi.SteamUser.UserInventory.Models.Response GetUserInventory(string steamID)
        {
            var bpUser = new BackpackTfUser(SharedData.Tokens.SteamID, SharedData.Tokens.BackpackToken, SharedData.Tokens.BackpackAccess);
            var results = bpUser.GetUserInventory(steamID);
            return results;
        }

        /// <summary>
        /// Fetches the current user's inventory.
        /// </summary>
        public static BackpackTfApi.SteamUser.UserInventory.Models.Response GetOwnInventory()
        {
            var bpUser = new BackpackTfUser(SharedData.Tokens.SteamID, SharedData.Tokens.BackpackToken, SharedData.Tokens.BackpackAccess);
            var results = bpUser.GetOwnInventory();
            return results;
        }

        /// <summary>
        /// Searches for an item in the user's inventory and returns its Asset and Description models.
        /// </summary>
        /// <param name="itemName"></param>
        public static InventoryItem GetItemFromInventory(string itemName)
        {
            var bpUser = new BackpackTfUser(SharedData.Tokens.SteamID, SharedData.Tokens.BackpackToken, SharedData.Tokens.BackpackAccess);
            var results = bpUser.GetItemFromInventory(itemName);
            return results;
        }

        #endregion BACKPACK.TF

        public static async Task<bool> UpdateTF2SchemaAsync()
        {
            try
            {
                var client = new EconItems(SharedData.Tokens.SteamToken, EconItemsAppId.TeamFortress2);
                var schema = (await client.GetSchemaForTF2Async()).Data;
                SharedData.TF2ItemSchema.Clear();
                foreach (var item in schema.Items)
                    if (!string.IsNullOrWhiteSpace(item.Name))
                        SharedData.TF2ItemSchema.Add(Convert.ToUInt32(item.DefIndex), item);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating TF2 item schema. " + ex.Message);
                return false;
            }
        }

        public static string TestTF2ItemSchema()
        {
            try
            {
                var econ = new EconItems(SharedData.Tokens.SteamToken, EconItemsAppId.TeamFortress2);
                var schema = econ.GetSchemaForTF2Async().Result;
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occured: {0}", ex.Message);
                return ex.Message;
            }
        }
    }
}