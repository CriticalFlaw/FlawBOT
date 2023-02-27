using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class OmdbModule : ApplicationCommandModule
    {
        [SlashCommand("omdb", "Find a movie or TV show from OMDB.")]
        public async Task Omdb(InteractionContext ctx, [Option("search", "Movie or TV show to find on OMDB.")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = OmdbService.GetMovieDataAsync(Program.Settings.Tokens.OmdbToken, query).Result;
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

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
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }
    }
}