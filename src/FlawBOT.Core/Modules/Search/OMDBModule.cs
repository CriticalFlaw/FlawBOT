using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class OMDBModule : BaseCommandModule
    {
        #region COMMAND_OMDB

        [Command("omdb")]
        [Aliases("imdb", "movie")]
        [Description("Retrieve a movie or TV show from OMDB")]
        public async Task OMDB(CommandContext ctx,
            [Description("Movie or TV show to find on OMDB")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = OMDBService.GetMovieDataAsync(query.Replace(" ", "+")).Result;
            if (results.Response == "False")
            {
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing)
                    .ConfigureAwait(false);
            }
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.Title)
                    .WithDescription(results.Plot.Length < 500 ? results.Plot : results.Plot.Take(500) + "...")
                    .AddField("Released", results.Released, true)
                    .AddField("Runtime", results.Runtime, true)
                    .AddField("Genre", results.Genre, true)
                    .AddField("Rating", results.Rated, true)
                    .AddField("Country", results.Country, true)
                    .AddField("Box Office", results.BoxOffice, true)
                    .AddField("Production", results.Production, true)
                    .AddField("IMDb Rating", results.IMDbRating, true)
                    .AddField("Metacritic", results.Metascore, true)
                    .AddField("Directors", results.Director)
                    .AddField("Actors", results.Actors)
                    .WithColor(DiscordColor.Goldenrod);
                if (results.Poster != "N/A") output.WithImageUrl(results.Poster);
                await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_OMDB
    }
}