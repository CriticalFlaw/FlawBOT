using System.Collections.Generic;
using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Games;

namespace FlawBOT.Framework.Services
{
    public class TwitchService : HttpHandler
    {
        public static async Task<TwitchData> GetTwitchDataAsync(string query)
        {
            var results = await _http.GetStringAsync(Resources.API_Twitch + query + "?client_id=" + TokenHandler.Tokens.TwitchToken);
            return JsonConvert.DeserializeObject<TwitchData>(results);
        }

        public static async Task<GetGamesResponse> GetTwitchGameAsync(string query)
        {
            var client = new TwitchAPI();
            client.Settings.ClientId = TokenHandler.Tokens.TwitchToken;
            var games = new List<string> { query };
            return await client.Helix.Games.GetGamesAsync(gameIds: games);
        }
    }
}