using System.Threading.Tasks;
using DSharpPlus.Entities;
using FlawBOT.Common;
using TwitchLib.Api;

namespace FlawBOT.Services
{
    public class TwitchService : HttpHandler
    {
        public static async Task<DiscordEmbed> GetTwitchDataAsync(string clientId, string accessToken, string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                var service = new TwitchAPI
                {
                    Settings =
                    {
                        ClientId = clientId,
                        AccessToken = accessToken,
                    }
                };
                var response = await service.Helix.Streams.GetStreamsAsync(query).ConfigureAwait(false);
                if (response.Streams.Length == 0) return null;
                var results = response.Streams[random.Next(response.Streams.Length)];

                // TODO: Add pagination when supported for slash commands.
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.Title)
                    .WithDescription("[LIVE] Now Playing: " + results.GameName)
                    .AddField("Broadcaster", results.Type.ToUpperInvariant(), true)
                    .AddField("Viewers", results.ViewerCount.ToString(), true)
                    .AddField("Started at", results.StartedAt.ToString())
                    .WithImageUrl(results.ThumbnailUrl)
                    .WithUrl($"https://www.twitch.tv/{results.UserName}")
                    .WithColor(new DiscordColor("#6441A5"));
                return output.Build();
            }
            catch
            {
                return null;
            }
        }
    }
}