using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class OmdbModule : BaseCommandModule
    {
        #region COMMAND_OMDB

        [Command("omdb"), Aliases("imdb", "movie")]
        [Description("Retrieve a movie or TV show from OMDB")]
        public async Task Omdb(CommandContext ctx,
            [Description("Movie or TV show to find on OMDB"), RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = OmdbService.GetMovieListAsync(query.Replace(" ", "+")).Result;
            if (!results.Search.Any())
            {
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_COMMON, EmbedType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            foreach (var title in results.Search)
            {
                var movie = OmdbService.GetMovieDataAsync(title.Title.Replace(" ", "+")).Result;
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
                        ? "Type 'next' within 10 seconds for the next movie"
                        : "This is the last found movie on OMDB.")
                    .WithColor(DiscordColor.Goldenrod);
                if (movie.Poster != "N/A") output.WithImageUrl(movie.Poster);
                var message = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                if (results.Search.Length == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                if (!movie.Title.Equals(results.Search.Last().Title))
                    await BotServices.RemoveMessage(message).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_OMDB
    }
}