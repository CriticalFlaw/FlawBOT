using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlawBOT.Services.Search
{
    public class SimpsonsService : HttpHandler
    {
        public static async Task<DiscordEmbedBuilder> GetSimpsonsDataAsync(string site)
        {
            var result = await _http.GetStringAsync($"https://{site}.com/api/random");
            var results = JsonConvert.DeserializeObject<SimpsonsData>(result);
            return EmbedSimpsonsEpisode(results, site);
        }

        public static async Task<string> GetSimpsonsGifAsync(string site)
        {
            var result = await _http.GetStringAsync($"https://{site}.com/api/random");
            var content = JsonConvert.DeserializeObject<SimpsonsData>(result);
            var frames_result = await _http.GetStringAsync($"https://{site}.com/api/frames/{content.Episode.Key}/{content.Frame.Timestamp}/3000/4000");
            var frames = JsonConvert.DeserializeObject<List<Frame>>(frames_result);
            var start = frames[0].Timestamp;
            var end = frames[frames.Count - 1].Timestamp;
            return $"https://{site}.com/gif/{content.Episode.Key}/{start}/{end}.gif";
        }

        public static DiscordEmbedBuilder EmbedSimpsonsEpisode(SimpsonsData data, string site)
        {
            var output = new DiscordEmbedBuilder()
                .WithTitle(data.Episode.Title)
                .AddField("Season/Episode", data.Episode.Key, true)
                .AddField("Air Date", data.Episode.OriginalAirDate, true)
                .AddField("Writer", data.Episode.Writer, true)
                .AddField("Director", data.Episode.Director, true)
                .WithImageUrl($"https://{site}.com/img/{data.Frame.Episode}/{data.Frame.Timestamp}.jpg")
                .WithColor(new DiscordColor("#FFBB22"))
                .WithUrl(data.Episode.WikiLink);
            return output;
        }
    }
}