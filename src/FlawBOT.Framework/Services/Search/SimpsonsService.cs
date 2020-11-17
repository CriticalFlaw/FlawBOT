using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
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
            var output = await Http.GetStringAsync(string.Format(Resources.URL_GIFS_Random, site))
                .ConfigureAwait(false);
            var results = JsonConvert.DeserializeObject<SimpsonsData>(output);
            return EmbedSimpsonsEpisode(results, site);
        }

        public static async Task<string> GetSimpsonsGifAsync(SiteRoot site)
        {
            var result = await Http.GetStringAsync(string.Format(Resources.URL_GIFS_Random, site))
                .ConfigureAwait(false);
            var content = JsonConvert.DeserializeObject<SimpsonsData>(result);
            var framesResult = await Http
                .GetStringAsync(string.Format(Resources.URL_GIFS_Frames, site, content.Episode.Key,
                    content.Frame.Timestamp))
                .ConfigureAwait(false);
            var frames = JsonConvert.DeserializeObject<List<Frame>>(framesResult);
            var start = frames.FirstOrDefault()?.Timestamp;
            var end = frames.LastOrDefault()?.Timestamp;
            return string.Format(Resources.URL_GIFS_Result, site, content.Episode.Key, start, end);
        }

        private static DiscordEmbedBuilder EmbedSimpsonsEpisode(SimpsonsData data, SiteRoot site)
        {
            var output = new DiscordEmbedBuilder()
                .WithTitle($"{data.Episode.Title} ({data.Episode.Key})")
                .AddField("Original Air Date", data.Episode.OriginalAirDate, true)
                .AddField("Writer", !string.IsNullOrWhiteSpace(data.Episode.Writer) ? data.Episode.Writer : "Unknown",
                    true)
                .AddField("Director",
                    !string.IsNullOrWhiteSpace(data.Episode.Director) ? data.Episode.Director : "Unknown", true)
                .WithImageUrl($"https://{site}.com/img/{data.Frame.Episode}/{data.Frame.Timestamp}.jpg")
                .WithColor(new DiscordColor("#FFBB22"))
                .WithUrl(data.Episode.WikiLink);
            return output;
        }
    }
}