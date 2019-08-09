using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackpackTfApi;
using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Services
{
    public class TeamFortressService : HttpHandler
    {
        public static Dictionary<uint, SchemaItem> ItemSchema { get; set; } = new Dictionary<uint, SchemaItem>();

        public static SchemaItem GetSchemaItemAsync(string query)
        {
            return ItemSchema.FirstOrDefault(n => n.Value.ItemName.ToLowerInvariant().Contains(query.ToLowerInvariant())).Value;
        }

        public static async Task<bool> LoadTF2SchemaAsync()
        {
            try
            {
                var schema = await _http.GetStringAsync(Resources.API_TradeTF + "?key=" + TokenHandler.Tokens.BackpackSchema);
                var results = JsonConvert.DeserializeObject<TFItemSchema>(schema);
                ItemSchema.Clear();
                foreach (var item in results.Results.Items)
                    if (!string.IsNullOrWhiteSpace(item.ItemName))
                        ItemSchema.Add(Convert.ToUInt32(item.DefIndex), item);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating TF2 item schema. " + ex.Message);
                return false;
            }
        }

        #region TEAMWORK.TF

        public static async Task<List<TeamworkNews>> GetNewsOverviewAsync()
        {
            var results = await _http.GetStringAsync(Resources.API_TeamworkTF + "news?key=" + TokenHandler.Tokens.TeamworkToken);
            return JsonConvert.DeserializeObject<List<TeamworkNews>>(results);
        }

        public static async Task<List<TeamworkServer>> GetServersAsync(string query)
        {
            var results = await _http.GetStringAsync(Resources.API_TeamworkTF + "quickplay/" + query + "/servers?key=" + TokenHandler.Tokens.TeamworkToken);
            return JsonConvert.DeserializeObject<List<TeamworkServer>>(results);
        }

        public static async Task<TeamworkMap> GetMapStatsAsync(string query)
        {
            var results = await _http.GetStringAsync(Resources.API_TeamworkTF + "map-stats/map/" + query + "?key=" + TokenHandler.Tokens.TeamworkToken);
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
        /// Fetches all currently open classifieds that are on backpack.tf
        /// </summary>
        public static BackpackTfApi.UserToken.Classifieds.ClassifiedsSearch.Models.Response GetClassifieds()
        {
            return new BackpackTfUser(TokenHandler.Tokens.SteamID, TokenHandler.Tokens.BackpackToken, TokenHandler.Tokens.BackpackAccess).GetClassifieds();
        }

        /// <summary>
        /// Fetches the currently opened user's classifieds from backpack.tf
        /// </summary>
        public static BackpackTfApi.UserToken.Classifieds.UserListings.Models.Response GetOwnClassifieds(string steamId)
        {
            return new BackpackTfUser(steamId, TokenHandler.Tokens.BackpackToken, TokenHandler.Tokens.BackpackAccess).GetOwnClassifieds();
        }

        #endregion BACKPACK.TF
    }
}