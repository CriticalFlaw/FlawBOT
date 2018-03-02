using DSharpPlus.Entities;
using Google.Apis.Services;
using Google.Apis.Urlshortener.v1;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class ShortenService
    {
        private UrlshortenerService shorten { get; set; }

        public ShortenService()
        {
            APITokenService service = new APITokenService();
            string Token = service.GetAPIToken("google");
            shorten = new UrlshortenerService(new BaseClientService.Initializer()
            {
                ApiKey = Token,
                ApplicationName = "FlawBOT",
            });
        }

        public string ShortenURL(string query)
        {
            var shorter = new Google.Apis.Urlshortener.v1.Data.Url();
            shorter.LongUrl = query;
            return shorten.Url.Insert(shorter).Execute().Id;
        }
    }

    public class YoutubeService
    {
        private YouTubeService youtube { get; set; }

        public YoutubeService()
        {
            APITokenService service = new APITokenService();
            string Token = service.GetAPIToken("google");
            youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Token,
                ApplicationName = "FlawBOT"
            });
        }

        public async Task<string> GetFirstVideoResultAsync(string query)
        {
            var res = await GetResultsAsync(query, 1, "video").ConfigureAwait(false);
            return $"https://www.youtube.com/watch?v={res.First().Id.VideoId}";
        }

        public async Task<DiscordEmbed> GetEmbeddedResults(string query, int amount, string type = null)
        {
            var results = await GetResultsAsync(query, amount, type).ConfigureAwait(false);
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
                        output.AddField(r.Snippet.Title, $"https://www.youtube.com/watch?v={r.Id.VideoId}");
                        break;

                    case "youtube#channel":
                        output.AddField(r.Snippet.Title, $"https://www.youtube.com/channel/{r.Id.ChannelId}");
                        break;

                    case "youtube#playlist":
                        output.AddField(r.Snippet.Title, $"https://www.youtube.com/playlist?list={r.Id.PlaylistId}");
                        break;
                }
            }
            return output.Build();
        }

        private async Task<List<SearchResult>> GetResultsAsync(string query, int amount, string type = null)
        {
            var searchListRequest = youtube.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = amount;
            if (type != null)
                searchListRequest.Type = type;
            var searchListResponse = await searchListRequest.ExecuteAsync().ConfigureAwait(false);
            List<SearchResult> videos = new List<SearchResult>();
            videos.AddRange(searchListResponse.Items);
            return videos;
        }
    }
}