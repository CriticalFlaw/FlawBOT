using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.V5.Models.Search;

namespace FlawBOT.Services.Lookup
{
    public class TwitchService : HttpHandler
    {
        public static async Task<SearchStreams> GetTwitchDataAsync(string token, string query)
        {
            var service = new TwitchAPI
            {
                Settings =
                {
                    ClientId = token
                }
            };
            return await service.V5.Search.SearchStreamsAsync(query).ConfigureAwait(false);
        }
    }
}