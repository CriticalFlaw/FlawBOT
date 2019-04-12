using FlawBOT.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Games;

namespace FlawBOT.Services.Search
{
    public class TwitchService
    {
        private static readonly string base_url = "https://api.twitch.tv/kraken/streams/";
        private static readonly HttpClient http = new HttpClient();

        public static async Task<TwitchData> GetTwitchDataAsync(string query)
        {
            //var client = new TwitchAPI();
            //client.Settings.ClientId = GlobalVariables.config.TwitchToken;
            //return await client.V5.Streams.GetLiveStreamsAsync(query);

            var results = await http.GetStringAsync(base_url + query + "?client_id=" + GlobalVariables.config.TwitchToken);
            return JsonConvert.DeserializeObject<TwitchData>(results);
        }

        public static async Task<GetGamesResponse> GetTwitchGameAsync(string query)
        {
            var client = new TwitchAPI();
            client.Settings.ClientId = GlobalVariables.config.TwitchToken;
            var games = new List<string> { query };
            return await client.Helix.Games.GetGamesAsync(gameIds: games);
        }
    }
}