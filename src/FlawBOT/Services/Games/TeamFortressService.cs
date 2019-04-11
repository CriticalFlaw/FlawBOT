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
            var rnd = new Random();
            var schema = new EconItems(GlobalVariables.config.SteamToken, EconItemsAppId.TeamFortress2);
            var items = await schema.GetSchemaForTF2Async();
            var index = rnd.Next(0, items.Data.Items.Count);
            return items.Data.Items[index];
        }

        public static async Task<WikipediaData> GetWikiPageAsync(string query)
        {
            var data = await http.GetStringAsync(wiki_url + query.Replace(' ', '_'));
            return JsonConvert.DeserializeObject<WikipediaData>(data);
        }

        public static async Task<List<TeamworkNews>> GetNewsOverviewAsync()
        {
            var token = GlobalVariables.config.TeamworkToken;
            var data = await http.GetStringAsync(news_url + $"?key={token}");
            return JsonConvert.DeserializeObject<List<TeamworkNews>>(data);
        }

        public static async Task<List<TeamworkServer>> GetServersAsync(string query)
        {
            switch (query.ToUpperInvariant())
            {
                case "AD":
                case "ATTACK-DEFENSE":
                    query = "attack-defend";
                    break;

                case "CAPTURE-THE-FLAG":
                    query = "ctf";
                    break;

                case "CP":
                    query = "control-point";
                    break;

                case "KING OF THE HILL":
                    query = "koth";
                    break;

                case "MANN VS MACHINE":
                    query = "mvm";
                    break;

                case "PL":
                    query = "payload";
                    break;

                case "PLR":
                    query = "payload-race";
                    break;

                default:
                    break;
            }

            var token = GlobalVariables.config.TeamworkToken;
            var data = await http.GetStringAsync(servers_url + $"/{query}/servers?key={token}");
            return JsonConvert.DeserializeObject<List<TeamworkServer>>(data);
        }

        public static async Task<TeamworkMap> GetMapStatsAsync(string query)
        {
            var token = GlobalVariables.config.TeamworkToken;
            var data = await http.GetStringAsync(maps_url + $"/{query}?key={token}");
            return JsonConvert.DeserializeObject<TeamworkMap>(data);
        }
    }
}