using DSharpPlus.Entities;
using OMDbSharp;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public static class OmdbService
    {
        public static async Task<DiscordEmbed> GetOMDBDataAsync(string token, string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                query = query.ToLowerInvariant().Replace("&", "%26").Replace(" ", "+");
                var results = await new OMDbClient(token, false).GetItemByTitle(query).ConfigureAwait(false);

                // TODO: Add pagination when supported for slash commands.
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.Title)
                    .WithDescription(results.Plot.Length < 500 ? results.Plot : results.Plot.Take(500) + "...")
                    .AddField("Released", results.Released, true)
                    .AddField("Runtime", results.Runtime, true)
                    .AddField("Genre", results.Genre, true)
                    .AddField("Rating", results.Rated, true)
                    .AddField("IMDb Rating", results.IMDbRating, true)
                    .AddField("Box Office", results.BoxOffice, true)
                    .AddField("Directors", results.Director)
                    .AddField("Actors", results.Actors)
                    .WithColor(DiscordColor.Goldenrod);
                if (results.Poster != "N/A") output.WithImageUrl(results.Poster);
                return output.Build();
            }
            catch
            {
                return null;
            }
        }
    }
}