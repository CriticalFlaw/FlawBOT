using System.Threading.Tasks;
using FlawBOT.Common;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;

namespace FlawBOT.Services
{
    public class TwitchService : HttpHandler
    {
        public static async Task<Stream> GetTwitchDataAsync(string clientId, string accessToken, string query)
        {
            var service = new TwitchAPI
            {
                Settings =
                {
                    ClientId = clientId,
                    AccessToken = accessToken,
                }
            };
            var result = await service.Helix.Streams.GetStreamsAsync(query).ConfigureAwait(false);
            if (result.Streams.Length == 0) return null;
            return result.Streams[random.Next(result.Streams.Length)];
        }
    }
}