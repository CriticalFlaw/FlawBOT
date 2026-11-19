using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;

namespace FlawBOT.Services
{
    public class TwitchService : HttpHandler
    {
        public static async Task<DiscordEmbed> GetTwitchDataAsync(string clientId, string accessToken, string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(query)) return null;
                var service = new TwitchAPI
                {
                    Settings =
                    {
                        ClientId = clientId,
                        AccessToken = accessToken
                    }
                };

                // Look up the stream by channel login. The first positional parameter is a pagination cursor, not a channel name.
                var login = query.Trim().TrimStart('@').ToLowerInvariant();
                var response = await service.Helix.Streams.GetStreamsAsync(userLogins: new List<string> { login }).ConfigureAwait(false);
                return response?.Streams is { Length: > 0 } streams ? BuildStreamEmbed(streams[0]) : null;
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed BuildStreamEmbed(Stream stream)
        {
            var output = new DiscordEmbedBuilder()
                .WithTitle(stream.Title.Truncate(EmbedExtensions.TitleLimit))
                .WithDescription("[LIVE] Now Playing: " + (string.IsNullOrWhiteSpace(stream.GameName) ? EmbedExtensions.Unknown : stream.GameName))
                .AddSafeField("Broadcaster", stream.UserName, true)
                .AddSafeField("Viewers", stream.ViewerCount.ToString(CultureInfo.InvariantCulture), true)
                .AddSafeField("Started at", stream.StartedAt.ToUniversalTime().ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture))
                .WithUrl(string.Format(Resources.URL_Twitch_Channel, stream.UserLogin))
                .WithColor(new DiscordColor("#6441A5"));
            if (!string.IsNullOrWhiteSpace(stream.ThumbnailUrl))
                output.WithImageUrl(stream.ThumbnailUrl.Replace("{width}", "1280", StringComparison.Ordinal).Replace("{height}", "720", StringComparison.Ordinal));
            return output.Build();
        }
    }
}
