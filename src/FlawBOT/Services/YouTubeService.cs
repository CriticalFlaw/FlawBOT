using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class YoutubeService : HttpHandler
    {
        public enum YouTubeSearch
        {
            Channel,
            Playlist,
            Video
        }

        public YoutubeService()
        {
            YouTube = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = Program.Settings.Tokens.YouTubeToken,
                ApplicationName = Program.Settings.Name
            });
        }

        private YouTubeService YouTube { get; }

        public async Task<string> GetFirstVideoResultAsync(string query)
        {
            var results = await GetResultsAsync(query, 1, YouTubeSearch.Video).ConfigureAwait(false);
            var video = results?.FirstOrDefault()?.Id?.VideoId;
            return video is null ? null : string.Format(Resources.URL_YouTube_Video, video);
        }

        public async Task<DiscordEmbed> GetEmbeddedResults(string query, int amount, YouTubeSearch type = YouTubeSearch.Video)
        {
            var results = await GetResultsAsync(query, amount, type).ConfigureAwait(false);
            return BuildResultsEmbed(results);
        }

        public static DiscordEmbed BuildResultsEmbed(IEnumerable<SearchResult> results)
        {
            var output = new DiscordEmbedBuilder { Color = DiscordColor.Red };
            foreach (var result in results?.Take(25) ?? [])
            {
                var link = result.Id?.Kind switch
                {
                    "youtube#video" => string.Format(Resources.URL_YouTube_Video, result.Id.VideoId),
                    "youtube#channel" => string.Format(Resources.URL_YouTube_Channel, result.Id.ChannelId),
                    "youtube#playlist" => string.Format(Resources.URL_YouTube_Playlist, result.Id.PlaylistId),
                    _ => null
                };
                if (link is not null)
                    output.AddSafeField(WebUtility.HtmlDecode(result.Snippet?.Title), link);
            }

            return output.Fields.Count == 0 ? null : output.Build();
        }

        private async Task<List<SearchResult>> GetResultsAsync(string query, int amount, YouTubeSearch type)
        {
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(YouTube.ApiKey)) return null;
            try
            {
                var searchListRequest = YouTube.Search.List("snippet");
                searchListRequest.Q = query.Trim();
                searchListRequest.MaxResults = amount;
                // Without a type filter, channel and playlist searches returned a mix of everything.
                searchListRequest.Type = type.ToString().ToLowerInvariant();
                var searchListResponse = await searchListRequest.ExecuteAsync().ConfigureAwait(false);
                return searchListResponse.Items?.ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}
