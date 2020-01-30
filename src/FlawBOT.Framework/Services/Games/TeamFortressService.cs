using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Models.GameEconomy;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        #region TEAMWORK.TF

        public static async Task<List<TeamworkNews>> GetNewsOverviewAsync()
        {
            var results = await _http.GetStringAsync(Resources.API_TeamworkTF + "news?key=" + TokenHandler.Tokens.TeamworkToken).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<TeamworkNews>>(results);
        }

        public static async Task<List<TeamworkServer>> GetServersAsync(string query)
        {
            try
            {
                var results = await _http.GetStringAsync(Resources.API_TeamworkTF + "quickplay/" + query + "/servers?key=" + TokenHandler.Tokens.TeamworkToken).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<List<TeamworkServer>>(results);
            }
            catch
            {
                return new List<TeamworkServer>();
            }
        }

        public static async Task<TeamworkMap> GetMapStatsAsync(string query)
        {
            try
            {
                query = NormalizedMapName(query).FirstOrDefault();
                var results = await _http.GetStringAsync(Resources.API_TeamworkTF + "map-stats/map/" + query + "?key=" + TokenHandler.Tokens.TeamworkToken).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<TeamworkMap>(results);
            }
            catch
            {
                return new TeamworkMap();
            }
        }

        public static string NormalizedGameMode(string input)
        {
            switch (input.ToUpperInvariant())
            {
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

                default:
                    return input;
            }
        }

        public static List<string> NormalizedMapName(string query)
        {
            query = (query.Contains('_')) ? query.ToLowerInvariant().Split('_')[1] : query;
            return mapList.Where(x => x.Contains(query)).ToList();
        }

        private static readonly List<string> mapList = new List<string>
        {
            "arena_badlands",
            "arena_byre",
            "arena_granary",
            "arena_lumberyard",
            "arena_nucleus",
            "arena_offblast_final",
            "arena_ravine",
            "arena_sawmill",
            "arena_watchtower",
            "arena_well",
            "cp_5gorge",
            "cp_badlands",
            "cp_cloak",
            "cp_coldfront",
            "cp_degrootkeep",
            "cp_dustbowl",
            "cp_egypt_final",
            "cp_fastlane",
            "cp_foundry",
            "cp_freight_final1",
            "cp_gorge",
            "cp_gorge_event",
            "cp_granary",
            "cp_gravelpit",
            "cp_gullywash_final1",
            "cp_junction_final",
            "cp_manor_event",
            "cp_mercenarypark",
            "cp_metalworks",
            "cp_mossrock",
            "cp_mountainlab",
            "cp_powerhouse",
            "cp_process_final",
            "cp_snakewater_final1",
            "cp_snowplow",
            "cp_standin_final",
            "cp_steel",
            "cp_sunshine",
            "cp_sunshine_event",
            "cp_vanguard",
            "cp_well",
            "cp_yukon_final",
            "ctf_2fort",
            "ctf_2fort_invasion",
            "ctf_doublecross",
            "ctf_foundry",
            "ctf_gorge",
            "ctf_hellfire",
            "ctf_landfall",
            "ctf_sawmill",
            "ctf_thundermountain",
            "ctf_turbine",
            "ctf_well",
            "itemtest",
            "koth_badlands",
            "koth_bagel_event",
            "koth_brazil",
            "koth_harvest_event",
            "koth_harvest_final",
            "koth_highpass",
            "koth_king",
            "koth_lakeside_event",
            "koth_lakeside_final",
            "koth_lazarus",
            "koth_maple_ridge_event",
            "koth_moonshine_event",
            "koth_nucleus",
            "koth_probed",
            "koth_sawmill",
            "koth_slasher",
            "koth_suijin",
            "koth_viaduct",
            "koth_viaduct_event",
            "mvm_bigrock",
            "mvm_coaltown",
            "mvm_decoy",
            "mvm_example",
            "mvm_ghost_town",
            "mvm_mannhattan",
            "mvm_mannworks",
            "mvm_rottenburg",
            "pass_brickyard",
            "pass_district",
            "pass_timbertown",
            "pd_cursed_cove_event",
            "pd_monster_bash",
            "pd_pit_of_death_event",
            "pd_watergate",
            "pl_badwater",
            "pl_barnblitz",
            "pl_borneo",
            "pl_cactuscanyon",
            "pl_enclosure_final",
            "pl_fifthcurve_event",
            "pl_frontier_final",
            "pl_goldrush",
            "pl_hoodoo_final",
            "pl_millstone_event",
            "pl_rumble_event",
            "pl_snowycoast",
            "pl_swiftwater_final1",
            "pl_thundermountain",
            "pl_upward",
            "plr_bananabay",
            "plr_hightower",
            "plr_hightower_event",
            "plr_nightfall_final",
            "plr_pipeline",
            "rd_asteroid",
            "sd_doomsday",
            "sd_doomsday_event",
            "tc_hydro",
            "tr_dustbowl",
            "tr_target"
        };

        #endregion TEAMWORK.TF
    }
}