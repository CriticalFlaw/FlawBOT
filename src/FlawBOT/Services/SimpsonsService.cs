using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Images;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class SimpsonsService : HttpHandler
    {
        public enum SiteRoot
        {
            Frinkiac,
            Morbotron
        }

        public static async Task<DiscordEmbed> GetEpisodeGifAsync(SiteRoot site)
        {
            try
            {
                var result = await Http.GetStringAsync(string.Format(Resources.URL_Simpsons_Random, site)).ConfigureAwait(false);
                var content = JsonConvert.DeserializeObject<SimpsonsData>(result);
                var framesResult = await Http.GetStringAsync(string.Format(Resources.URL_Simpsons_Frames, site, content?.Episode.Key, content?.Frame.Timestamp)).ConfigureAwait(false);
                var frames = JsonConvert.DeserializeObject<List<Frame>>(framesResult);
                var start = frames?.FirstOrDefault()?.Timestamp;
                var end = frames?.LastOrDefault()?.Timestamp;
                var image = string.Format(Resources.URL_Simpsons_Result, site, content.Episode.Key, start, end);

                var output = new DiscordEmbedBuilder()
                    .WithTitle($"{content.Episode.Title} ({content.Episode.Key})")
                    .AddField("Original Air Date", content.Episode.OriginalAirDate, true)
                    .AddField("Writer", !string.IsNullOrWhiteSpace(content.Episode.Writer) ? content.Episode.Writer : "Unknown", true)
                    .AddField("Director", !string.IsNullOrWhiteSpace(content.Episode.Director) ? content.Episode.Director : "Unknown", true)
                    .WithImageUrl(image ?? string.Format(Resources.URL_Simpsons_Image, site, content.Frame.Episode, content.Frame.Timestamp))
                    .WithFooter(Resources.INFO_GIF_LOADING)
                    .WithColor(new DiscordColor(site == SiteRoot.Frinkiac ? "#FFBB22" : "#69E398"))
                    .WithUrl(content.Episode.WikiLink);
                return output.Build();
            }
            catch
            {
                return null;
            }
        }
    }
}