using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.OMDB;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class OmdbService : HttpHandler
    {
        public static async Task<DiscordEmbed> GetOMDBDataAsync(string token, string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(query)) return null;
                var url = string.Format(Resources.URL_OMDB, Uri.EscapeDataString(query.Trim()), Uri.EscapeDataString(token));
                var response = await Http.GetStringAsync(url).ConfigureAwait(false);
                return BuildMovieEmbed(JsonConvert.DeserializeObject<OmdbData>(response));
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed BuildMovieEmbed(OmdbData results)
        {
            if (results is null || !string.Equals(results.Response, "True", StringComparison.OrdinalIgnoreCase)) return null;
            var output = new DiscordEmbedBuilder()
                .WithTitle($"{results.Title} ({results.Year})".Truncate(EmbedExtensions.TitleLimit))
                .AddSafeField("Released", NotAvailable(results.Released), true)
                .AddSafeField("Runtime", NotAvailable(results.Runtime), true)
                .AddSafeField("Genre", NotAvailable(results.Genre), true)
                .AddSafeField("Rating", NotAvailable(results.Rated), true)
                .AddSafeField("IMDb Rating", NotAvailable(results.ImdbRating), true)
                .AddSafeField("Box Office", NotAvailable(results.BoxOffice), true)
                .AddSafeField("Directors", NotAvailable(results.Director))
                .AddSafeField("Actors", NotAvailable(results.Actors))
                .WithColor(DiscordColor.Goldenrod);
            if (NotAvailable(results.Plot) is { } plot) output.WithDescription(plot.Truncate(500));
            if (NotAvailable(results.Poster) is { } poster) output.WithImageUrl(poster);
            if (!string.IsNullOrWhiteSpace(results.ImdbId)) output.WithUrl(string.Format(Resources.URL_IMDB, results.ImdbId));
            return output.Build();
        }

        /// <summary>
        ///     OMDB reports missing values as "N/A".
        /// </summary>
        private static string NotAvailable(string value)
        {
            return string.IsNullOrWhiteSpace(value) || value == "N/A" ? null : value;
        }
    }
}
