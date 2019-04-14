using FlawBOT.Common;
using FlawBOT.Models;
using Newtonsoft.Json;
using Steam.Models.TF2;
using SteamWebAPI2.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services.Games
{
    public class TeamFortressService
    {
        private static readonly string news_url = "https://teamwork.tf/api/v1/news";
        private static readonly string servers_url = "https://teamwork.tf/api/v1/quickplay";
        private static readonly string maps_url = "https://teamwork.tf/api/v1/map-stats/map";
        private static readonly string wiki_url = "https://wiki.teamfortress.com/w/api.php?action=query&format=json&prop=info&inprop=url&titles=";
        private static readonly HttpClient http = new HttpClient();

        public static async Task<SchemaItemModel> GetSchemaItemAsync()
        {
            var random = new Random();
            var schema = new EconItems(SharedData.Tokens.SteamToken, EconItemsAppId.TeamFortress2);
            var items = await schema.GetSchemaForTF2Async();
            var index = random.Next(0, items.Data.Items.Count);
            return items.Data.Items[index];
        }

        public static async Task<WikipediaData> GetWikiPageAsync(string query)
        {
            var results = await http.GetStringAsync(wiki_url + query.Replace(' ', '_'));
            return JsonConvert.DeserializeObject<WikipediaData>(results);
        }

        public static async Task<List<TeamworkNews>> GetNewsOverviewAsync()
        {
            var token = SharedData.Tokens.TeamworkToken;
            var results = await http.GetStringAsync(news_url + $"?key={token}");
            return JsonConvert.DeserializeObject<List<TeamworkNews>>(results);
        }

        public static async Task<List<TeamworkServer>> GetServersAsync(string query)
        {
            var token = SharedData.Tokens.TeamworkToken;
            var results = await http.GetStringAsync(servers_url + $"/{NormalizedGameMode(query)}/servers?key={token}");
            return JsonConvert.DeserializeObject<List<TeamworkServer>>(results);
        }

        public static async Task<TeamworkMap> GetMapStatsAsync(string query)
        {
            var token = SharedData.Tokens.TeamworkToken;
            var results = await http.GetStringAsync(maps_url + $"/{query}?key={token}");
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
    }
}