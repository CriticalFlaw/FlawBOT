using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Images;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class SimpsonsService : HttpHandler
    {
        public enum SiteRoot
        {
            Frinkiac,
            Morbotron
        }

        public static async Task<DiscordEmbedBuilder> GetEpisodeGifAsync(SiteRoot site)
        {
            var result = await Http.GetStringAsync(string.Format(Resources.URL_Simpsons_Random, site)).ConfigureAwait(false);
            var content = JsonConvert.DeserializeObject<SimpsonsData>(result);
            var framesResult = await Http.GetStringAsync(string.Format(Resources.URL_Simpsons_Frames, site, content?.Episode.Key, content?.Frame.Timestamp)).ConfigureAwait(false);
            var frames = JsonConvert.DeserializeObject<List<Frame>>(framesResult);
            var start = frames?.FirstOrDefault()?.Timestamp;
            var end = frames?.LastOrDefault()?.Timestamp;
            return EmbedEpisodeOutput(content, string.Format(Resources.URL_Simpsons_Result, site, content.Episode.Key, start, end), site);
        }

        private static DiscordEmbedBuilder EmbedEpisodeOutput(SimpsonsData data, string image, SiteRoot site)
        {
            var output = new DiscordEmbedBuilder()
                .WithTitle($"{data.Episode.Title} ({data.Episode.Key})")
                .AddField("Original Air Date", data.Episode.OriginalAirDate, true)
                .AddField("Writer", !string.IsNullOrWhiteSpace(data.Episode.Writer) ? data.Episode.Writer : "Unknown", true)
                .AddField("Director", !string.IsNullOrWhiteSpace(data.Episode.Director) ? data.Episode.Director : "Unknown", true)
                .WithImageUrl(image ?? string.Format(Resources.URL_Simpsons_Image, site, data.Frame.Episode, data.Frame.Timestamp))
                .WithFooter(Resources.INFO_GIF_LOADING)
                .WithColor(new DiscordColor(site == SiteRoot.Frinkiac ? "#FFBB22" : "#69E398"))
                .WithUrl(data.Episode.WikiLink);
            return output;
        }
    }
}