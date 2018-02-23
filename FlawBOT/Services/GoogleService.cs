using DSharpPlus.Entities;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class GoogleService
    {
        public class YoutubeService
        {
            private YouTubeService _yt { get; set; }
            private string _key { get; set; }

            public YoutubeService(string key)
            {
                _key = key;
                _yt = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = key,
                    ApplicationName = "FlawBOT"
                });
            }

            public static string GetYoutubeRSSFeedLinkForChannelId(string id) =>
                "https://www.youtube.com/feeds/videos.xml?channel_id=" + id;

            public async Task<string> GetFirstVideoResultAsync(string query)
            {
                var res = await GetResultsAsync(query, 1, "video").ConfigureAwait(false);
                return "https://www.youtube.com/watch?v=" + res.First().Id.VideoId;
            }

            public async Task<DiscordEmbed> GetEmbeddedResults(string query, int amount, string type = null)
            {
                var res = await GetResultsAsync(query, amount, type).ConfigureAwait(false);
                return EmbedYouTubeResults(res);
            }

            private async Task<List<SearchResult>> GetResultsAsync(string query, int amount, string type = null)
            {
                var searchListRequest = _yt.Search.List("snippet");
                searchListRequest.Q = query;
                searchListRequest.MaxResults = amount;
                if (type != null)
                    searchListRequest.Type = type;

                var searchListResponse = await searchListRequest.ExecuteAsync().ConfigureAwait(false);

                List<SearchResult> videos = new List<SearchResult>();
                videos.AddRange(searchListResponse.Items);

                return videos;
            }

            private DiscordEmbed EmbedYouTubeResults(List<SearchResult> results)
            {
                if (results == null || results.Count == 0)
                    return new DiscordEmbedBuilder() { Description = "No results...", Color = DiscordColor.Red };

                if (results.Count > 25)
                    results = results.Take(25).ToList();

                var output = new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Red);
                foreach (var r in results)
                {
                    switch (r.Id.Kind)
                    {
                        case "youtube#video":
                            output.AddField(r.Snippet.Title, "https://www.youtube.com/watch?v=" + r.Id.VideoId);
                            break;

                        case "youtube#channel":
                            output.AddField(r.Snippet.Title, "https://www.youtube.com/channel/" + r.Id.ChannelId);
                            break;

                        case "youtube#playlist":
                            output.AddField(r.Snippet.Title, "https://www.youtube.com/playlist?list=" + r.Id.PlaylistId);
                            break;
                    }
                }
                return output.Build();
            }

            private sealed class DeserializedData
            {
                [JsonProperty("items")]
                public List<Dictionary<string, string>> Items { get; set; }
            }
        }
    }
}