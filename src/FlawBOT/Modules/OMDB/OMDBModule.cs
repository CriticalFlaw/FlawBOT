using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class OmdbModule : ApplicationCommandModule
    {
        #region COMMAND_OMDB

        [SlashCommand("omdb", "Find a movie or TV show from OMDB.")]
        public async Task Omdb(InteractionContext ctx, [Option("query", "Movie or TV show to find on OMDB.")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = OmdbService.GetMovieListAsync(Program.Settings.Tokens.OmdbToken, query.Replace(" ", "+")).Result;
            if (!results.Search.Any())
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            foreach (var title in results.Search)
            {
                var movie = OmdbService.GetMovieDataAsync(Program.Settings.Tokens.OmdbToken, title.Title.Replace(" ", "+")).Result;
                var output = new DiscordEmbedBuilder()
                    .WithTitle(movie.Title)
                    .WithDescription(movie.Plot.Length < 500 ? movie.Plot : movie.Plot.Take(500) + "...")
                    .AddField("Released", movie.Released, true)
                    .AddField("Runtime", movie.Runtime, true)
                    .AddField("Genre", movie.Genre, true)
                    .AddField("Rating", movie.Rated, true)
                    .AddField("IMDb Rating", movie.IMDbRating, true)
                    .AddField("Box Office", movie.BoxOffice, true)
                    .AddField("Directors", movie.Director)
                    .AddField("Actors", movie.Actors)
                    .WithFooter(!movie.Title.Equals(results.Search.Last().Title)
                        ? "Type 'next' within 10 seconds for the next movie."
                        : "This is the last found movie on OMDB.")
                    .WithColor(DiscordColor.Goldenrod);
                if (movie.Poster != "N/A") output.WithImageUrl(movie.Poster);
                await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);

                if (results.Search.Length == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_OMDB
    }
}