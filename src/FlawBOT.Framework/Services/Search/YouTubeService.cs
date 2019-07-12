using DSharpPlus.Entities;
using FlawBOT.Framework.Common;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class YoutubeService
    {
        private YouTubeService YouTube { get; }

        public YoutubeService()
        {
            YouTube = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = TokenHandler.Tokens.GoogleToken,
                ApplicationName = "FlawBOT"
            });
        }

        public async Task<string> GetFirstVideoResultAsync(string query)
        {
            var results = await GetResultsAsync(query, 1, "video").ConfigureAwait(false);
            if (results == null || results.Count == 0) return ":warning: No results found!";
            return "https://www.youtube.com/watch?v=" + results.First().Id.VideoId;
        }

        public async Task<DiscordEmbed> GetEmbeddedResults(string query, int amount, string type = null)
        {
            var results = await GetResultsAsync(query, amount, type).ConfigureAwait(false);
            if (results == null || results.Count == 0)
                return new DiscordEmbedBuilder
                {
                    Description = ":warning: No results found!",
                    Color = DiscordColor.Red
                };
            results = (results.Count > 25) ? results.Take(25).ToList() : results;
            var output = new DiscordEmbedBuilder { Color = DiscordColor.Red };
            foreach (var result in results)
                switch (result.Id.Kind)
                {
                    case "youtube#video":
                        output.AddField(result.Snippet.Title, "https://www.youtube.com/watch?v=" + result.Id.VideoId);
                        break;

                    case "youtube#channel":
                        output.AddField(result.Snippet.Title, "https://www.youtube.com/channel/" + result.Id.ChannelId);
                        break;

                    case "youtube#playlist":
                        output.AddField(result.Snippet.Title, "https://www.youtube.com/playlist?list=" + result.Id.PlaylistId);
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