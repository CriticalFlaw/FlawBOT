using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class TwitchService : HttpHandler
    {
        public static async Task<TwitchData> GetTwitchDataAsync(string query)
        {
            using var request = new HttpRequestMessage(new HttpMethod("GET"), Resources.API_Twitch + "streams?user_login=" + query);
            request.Headers.TryAddWithoutValidation("Client-ID", TokenHandler.Tokens.TwitchToken);
            var response = await _http.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var results = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<TwitchData>(results);
        }
    }
}