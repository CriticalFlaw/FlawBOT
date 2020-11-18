using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace FlawBOT.Framework.Services
{
    public class YoutubeService
    {
        public YoutubeService()
        {
            YouTube = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = TokenHandler.Tokens.YouTubeToken,
                ApplicationName = "FlawBOT"
            });
        }

        private YouTubeService YouTube { get; }

        public async Task<string> GetFirstVideoResultAsync(string query)
        {
            var results = await GetResultsAsync(query, 1, "video").ConfigureAwait(false);
            if (results is null || results.Count == 0) return ":warning: No results found!";
            return string.Format(Resources.URL_YouTube_Video, results.FirstOrDefault()?.Id.VideoId);
        }

        public async Task<DiscordEmbed> GetEmbeddedResults(string query, int amount, string type = null)
        {
            var results = await GetResultsAsync(query, amount, type).ConfigureAwait(false);
            if (results is null || results.Count == 0)
                return new DiscordEmbedBuilder
                {
                    Description = ":warning: No results found!",
                    Color = DiscordColor.Red
                };
            results = results.Count > 25 ? results.Take(25).ToList() : results;
            var output = new DiscordEmbedBuilder { Color = DiscordColor.Red };
            foreach (var result in results)
                switch (result.Id.Kind)
                {
                    case "youtube#video":
                        output.AddField(result.Snippet.Title,
                            string.Format(Resources.URL_YouTube_Video, result.Id.VideoId));
                        break;

                    case "youtube#channel":
                        output.AddField(result.Snippet.Title,
                            string.Format(Resources.URL_YouTube_Channel, result.Id.ChannelId));
                        break;

                    case "youtube#playlist":
                        output.AddField(result.Snippet.Title,
                            string.Format(Resources.URL_YouTube_Playlist, result.Id.PlaylistId));
                        break;

                    default:
                        return null;
                }

            return output.Build();
        }

        private async Task<List<SearchResult>> GetResultsAsync(string query, int amount, string type = null)
        {
            var searchListRequest = YouTube.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = amount;
            if (!string.IsNullOrWhiteSpace(type))
                searchListRequest.Type = type;
            var searchListResponse = await searchListRequest.ExecuteAsync().ConfigureAwait(false);
            var videos = new List<SearchResult>();
            videos.AddRange(searchListResponse.Items);
            return videos;
        }
    }
}