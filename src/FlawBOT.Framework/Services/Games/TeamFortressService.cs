using BackpackTfApi;
using BackpackTfApi.Economy.PriceHistory.Models;
using BackpackTfApi.Economy.Prices.Models;
using BackpackTfApi.Economy.SpecialItems;
using BackpackTfApi.SteamUser.UserInventory.Models;
using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class TeamFortressService : HttpHandler
    {
        public static Dictionary<uint, SchemaItem> ItemSchema { get; set; } = new Dictionary<uint, SchemaItem>();

        public static SchemaItem GetSchemaItemAsync(string query)
        {
            var results = ItemSchema.FirstOrDefault(n => n.Value.Name.Contains(query)).Value;
            return results;
        }

        #region TEAMWORK.TF

        public static async Task<List<TeamworkNews>> GetNewsOverviewAsync()
        {
            var results = await _http.GetStringAsync("https://teamwork.tf/api/v1/news?key=" + TokenHandler.Tokens.TeamworkToken);
            return JsonConvert.DeserializeObject<List<TeamworkNews>>(results);
        }

        public static async Task<List<TeamworkServer>> GetServersAsync(string query)
        {
            var results = await _http.GetStringAsync("https://teamwork.tf/api/v1/quickplay/" + query + "/servers?key=" + TokenHandler.Tokens.TeamworkToken);
            return JsonConvert.DeserializeObject<List<TeamworkServer>>(results);
        }

        public static async Task<TeamworkMap> GetMapStatsAsync(string query)
        {
            var results = await _http.GetStringAsync("https://teamwork.tf/api/v1/map-stats/map/" + query + "?key=" + TokenHandler.Tokens.TeamworkToken);
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
            var bpUser = new BackpackTfUser(TokenHandler.Tokens.SteamID, TokenHandler.Tokens.BackpackToken, TokenHandler.Tokens.BackpackAccess);
            var results = bpUser.GetPriceHistory(itemName, 1);
            return results;
        }

        /// <summary>
        /// Fetches item prices for the specified API key. A request may be sent once every 60 seconds.
        /// </summary>
        public static PricesData GetItemPrices()
        {
            var bpUser = new BackpackTfUser(TokenHandler.Tokens.SteamID, TokenHandler.Tokens.BackpackToken, TokenHandler.Tokens.BackpackAccess);
            var results = bpUser.GetItemPrices();
            return results;
        }

        /// <summary>
        /// Gets special items for the specified API key.
        /// </summary>
        public static SpecialItemsData GetSpecialItems()
        {
            var bpUser = new BackpackTfUser(TokenHandler.Tokens.SteamID, TokenHandler.Tokens.BackpackToken, TokenHandler.Tokens.BackpackAccess);
            var results = bpUser.GetSpecialItems();
            return results;
        }

        /// <summary>
        /// Fetches all currently open classifieds that are on backpack.tf
        /// </summary>
        public static BackpackTfApi.UserToken.Classifieds.ClassifiedsSearch.Models.Response GetClassifieds()
        {
            var bpUser = new BackpackTfUser(TokenHandler.Tokens.SteamID, TokenHandler.Tokens.BackpackToken, TokenHandler.Tokens.BackpackAccess);
            var results = bpUser.GetClassifieds();
            return results;
        }

        /// <summary>
        /// Fetches the currently opened user's classifieds from backpack.tf
        /// </summary>
        public static BackpackTfApi.UserToken.Classifieds.UserListings.Models.Response GetOwnClassifieds()
        {
            var bpUser = new BackpackTfUser(TokenHandler.Tokens.SteamID, TokenHandler.Tokens.BackpackToken, TokenHandler.Tokens.BackpackAccess);
            var results = bpUser.GetOwnClassifieds();
            return results;
        }

        /// <summary>
        /// Fetches a user's inventory.
        /// </summary>
        /// <param name="steamID"></param>
        public static BackpackTfApi.SteamUser.UserInventory.Models.Response GetUserInventory(string steamID)
        {
            var bpUser = new BackpackTfUser(TokenHandler.Tokens.SteamID, TokenHandler.Tokens.BackpackToken, TokenHandler.Tokens.BackpackAccess);
            var results = bpUser.GetUserInventory(steamID);
            return results;
        }

        /// <summary>
        /// Fetches the current user's inventory.
        /// </summary>
        public static BackpackTfApi.SteamUser.UserInventory.Models.Response GetOwnInventory()
        {
            var bpUser = new BackpackTfUser(TokenHandler.Tokens.SteamID, TokenHandler.Tokens.BackpackToken, TokenHandler.Tokens.BackpackAccess);
            var results = bpUser.GetOwnInventory();
            return results;
        }

        /// <summary>
        /// Searches for an item in the user's inventory and returns its Asset and Description models.
        /// </summary>
        /// <param name="itemName"></param>
        public static InventoryItem GetItemFromInventory(string itemName)
        {
            var bpUser = new BackpackTfUser(TokenHandler.Tokens.SteamID, TokenHandler.Tokens.BackpackToken, TokenHandler.Tokens.BackpackAccess);
            var results = bpUser.GetItemFromInventory(itemName);
            return results;
        }

        #endregion BACKPACK.TF

        public static async Task<bool> UpdateTF2SchemaAsync()
        {
            try
            {
                var schema = await _http.GetStringAsync("https://api.steampowered.com/IEconItems_440/GetSchemaItems/v0001/?key=" + TokenHandler.Tokens.SteamToken);
                var results = JsonConvert.DeserializeObject<TF2ItemSchema>(schema);
                ItemSchema.Clear();
                foreach (var item in results.Result.Items)
                    if (!string.IsNullOrWhiteSpace(item.Name))
                        ItemSchema.Add(Convert.ToUInt32(item.DefIndex), item);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating TF2 item schema. " + ex.Message);
                return false;
            }
        }
    }
}