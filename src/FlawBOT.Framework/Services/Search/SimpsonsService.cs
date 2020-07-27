using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using FlawBOT.Framework.Models;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Services
{
    public class SimpsonsService : HttpHandler
    {
        public enum SiteRoot
        {
            Frinkiac,
            Morbotron,
            MasterOfAllScience
        }

        public static async Task<DiscordEmbedBuilder> GetSimpsonsDataAsync(SiteRoot site)
        {
            var output = await Http.GetStringAsync($"https://{site}.com/api/random").ConfigureAwait(false);
            var results = JsonConvert.DeserializeObject<SimpsonsData>(output);
            return EmbedSimpsonsEpisode(results, site);
        }

        public static async Task<string> GetSimpsonsGifAsync(SiteRoot site)
        {
            var result = await Http.GetStringAsync($"https://{site}.com/api/random").ConfigureAwait(false);
            var content = JsonConvert.DeserializeObject<SimpsonsData>(result);
            var framesResult = await Http
                .GetStringAsync($"https://{site}.com/api/frames/{content.Episode.Key}/{content.Frame.Timestamp}/3000/4000")
                .ConfigureAwait(false);
            var frames = JsonConvert.DeserializeObject<List<Frame>>(framesResult);
            var start = frames[0].Timestamp;
            var end = frames[^1].Timestamp;
            return $"https://{site}.com/gif/{content.Episode.Key}/{start}/{end}.gif";
        }

        private static DiscordEmbedBuilder EmbedSimpsonsEpisode(SimpsonsData data, SiteRoot site)
        {
            var output = new DiscordEmbedBuilder()
                .WithTitle(data.Episode.Title)
                .AddField("Season/Episode", data.Episode.Key, true)
                .AddField("Writer", !string.IsNullOrWhiteSpace(data.Episode.Writer) ? data.Episode.Writer : "Unknown",true)
                .AddField("Director", !string.IsNullOrWhiteSpace(data.Episode.Director) ? data.Episode.Director : "Unknown", true)
                .WithFooter("Original Air Date: " + data.Episode.OriginalAirDate)
                .WithImageUrl($"https://{site}.com/img/{data.Frame.Episode}/{data.Frame.Timestamp}.jpg")
                .WithColor(new DiscordColor("#FFBB22"))
                .WithUrl(data.Episode.WikiLink);
            return output;
        }
    }
}