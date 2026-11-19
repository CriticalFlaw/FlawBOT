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

        public static async Task<DiscordEmbed> GetEpisodePostAsync(SiteRoot site)
        {
            try
            {
                var host = GetHost(site);
                var result = await Http.GetStringAsync(string.Format(Resources.URL_Simpsons_Random, host)).ConfigureAwait(false);
                var content = JsonConvert.DeserializeObject<SimpsonsData>(result);
                if (content?.Episode is null || content.Frame is null) return null;

                List<Frame> frames = null;
                try
                {
                    var framesResult = await Http.GetStringAsync(string.Format(Resources.URL_Simpsons_Frames, host, content.Episode.Key, content.Frame.Timestamp)).ConfigureAwait(false);
                    frames = JsonConvert.DeserializeObject<List<Frame>>(framesResult);
                }
                catch
                {
                    // Fall back to a still image below.
                }

                return BuildEpisodeEmbed(site, content, frames);
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed BuildEpisodeEmbed(SiteRoot site, SimpsonsData content, IReadOnlyList<Frame> frames)
        {
            var host = GetHost(site);
            var output = new DiscordEmbedBuilder()
                .WithTitle($"{content.Episode.Title} ({content.Episode.Key})")
                .AddSafeField("Original Air Date", content.Episode.OriginalAirDate, true)
                .AddSafeField("Writer", content.Episode.Writer, true)
                .AddSafeField("Director", content.Episode.Director, true)
                .WithColor(new DiscordColor(site == SiteRoot.Frinkiac ? "#FFBB22" : "#69E398"));

            // Use an animated GIF when the surrounding frames are known, otherwise a still of the random frame.
            if (frames is { Count: > 0 })
            {
                output.WithImageUrl(string.Format(Resources.URL_Simpsons_Result, host, content.Episode.Key, frames[0].Timestamp, frames[^1].Timestamp));
                output.WithFooter(Resources.INFO_GIF_LOADING);
            }
            else
                output.WithImageUrl(string.Format(Resources.URL_Simpsons_Image, host, content.Frame.Episode, content.Frame.Timestamp));

            if (!string.IsNullOrWhiteSpace(content.Episode.WikiLink)) output.WithUrl(content.Episode.WikiLink);
            return output.Build();
        }

        private static string GetHost(SiteRoot site)
        {
            return site.ToString().ToLowerInvariant();
        }
    }
}
