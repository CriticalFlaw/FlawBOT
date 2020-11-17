using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using TwitchLib.Api;
using TwitchLib.Api.V5.Models.Search;

namespace FlawBOT.Framework.Services
{
    public class TwitchService : HttpHandler
    {
        public static async Task<SearchStreams> GetTwitchDataAsync(string query)
        {
            var service = new TwitchAPI();
            service.Settings.ClientId = TokenHandler.Tokens.TwitchToken;
            return await service.V5.Search.SearchStreamsAsync(query).ConfigureAwait(false);
        }
    }
}