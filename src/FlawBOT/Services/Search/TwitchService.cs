using FlawBOT.Common;
using FlawBOT.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Games;

namespace FlawBOT.Services.Search
{
    public class TwitchService : HttpHandler
    {
        private static readonly string base_url = "https://api.twitch.tv/kraken/streams/";

        public static async Task<TwitchData> GetTwitchDataAsync(string query)
        {
            var results = await _http.GetStringAsync(base_url + query + "?client_id=" + SharedData.Tokens.TwitchToken);
            return JsonConvert.DeserializeObject<TwitchData>(results);
        }

        public static async Task<GetGamesResponse> GetTwitchGameAsync(string query)
        {
            var client = new TwitchAPI();
            client.Settings.ClientId = SharedData.Tokens.TwitchToken;
            var games = new List<string> { query };
            return await client.Helix.Games.GetGamesAsync(gameIds: games);
        }
    }
}