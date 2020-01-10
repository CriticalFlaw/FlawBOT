using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
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
            return ItemSchema.FirstOrDefault(n => n.Value.ItemName.ToLowerInvariant().Contains(query.ToLowerInvariant())).Value;
        }

        public static async Task<bool> LoadTF2SchemaAsync()
        {
            try
            {
                var schema = await _http.GetStringAsync(Resources.API_TradeTF + "?key=" + TokenHandler.Tokens.TFSchemaToken).ConfigureAwait(false);
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
            var results = await _http.GetStringAsync(Resources.API_TeamworkTF + "news?key=" + TokenHandler.Tokens.TeamworkToken).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<TeamworkNews>>(results);
        }

        public static async Task<List<TeamworkServer>> GetServersAsync(string query)
        {
            var results = await _http.GetStringAsync(Resources.API_TeamworkTF + "quickplay/" + query + "/servers?key=" + TokenHandler.Tokens.TeamworkToken).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<TeamworkServer>>(results);
        }

        public static async Task<TeamworkMap> GetMapStatsAsync(string query)
        {
            var results = await _http.GetStringAsync(Resources.API_TeamworkTF + "map-stats/map/" + NormalizedMapName(query) + "?key=" + TokenHandler.Tokens.TeamworkToken).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<TeamworkMap>(results);
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

        public static string NormalizedMapName(string input)
        {
            input = input.ToLowerInvariant().Split('_')[1];
            return mapList.Where(x => x.Contains(input)).Take(1).ToString();
        }

        #endregion TEAMWORK.TF

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
    }
}