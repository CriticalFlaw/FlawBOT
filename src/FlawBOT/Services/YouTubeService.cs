using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Music;
using FlawBOT.Properties;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
            if (string.IsNullOrWhiteSpace(query)) return null;
            var results = await GetResultsAsync(query, 1, YouTubeSearch.Video).ConfigureAwait(false);
            if (results is null || results.Count == 0) return Resources.NOT_FOUND_COMMON;
            return string.Format(Resources.URL_YouTube_Video, results.FirstOrDefault()?.Id.VideoId);
        }

        public async Task<DiscordEmbed> GetEmbeddedResults(string query, int amount, YouTubeSearch type = YouTubeSearch.Video)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;
            var results = await GetResultsAsync(query, amount, type).ConfigureAwait(false);
            if (results is null || results.Count == 0)
                return new DiscordEmbedBuilder
                {
                    Description = Resources.NOT_FOUND_COMMON,
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

        private async Task<List<SearchResult>> GetResultsAsync(string query, int amount, YouTubeSearch type = YouTubeSearch.Video)
        {
            var searchListRequest = YouTube.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = amount;
            var searchListResponse = await searchListRequest.ExecuteAsync().ConfigureAwait(false);
            var videos = new List<SearchResult>();
            videos.AddRange(searchListResponse.Items);
            return videos;
        }

        public async Task<IEnumerable<YouTubeData>> GetMusicDataAsync(string term)
        {
            var uri = new Uri($"https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=5&type=video&fields=items(id(videoId),snippet(title,channelTitle))&key={YouTube.ApiKey}&q={WebUtility.UrlEncode(term)}");

            string json;
            Http.BaseAddress = new Uri("https://www.googleapis.com/youtube/v3/search");
            Http.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", Program.Settings.Name);
            using (var req = await Http.GetAsync(uri))
            await using (var res = await req.Content.ReadAsStreamAsync())
            using (var sr = new StreamReader(res, Encoding.UTF8))
            {
                json = await sr.ReadToEndAsync();
            }

            var jsonData = JObject.Parse(json);
            var data = jsonData["items"]?.ToObject<IEnumerable<YouTubeResponse>>();

            return data!.Select(x => new YouTubeData(x.Snippet.Title, x.Snippet.Author, x.Id.VideoId));
        }
    }
}