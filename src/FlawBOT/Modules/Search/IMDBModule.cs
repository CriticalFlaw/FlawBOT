using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Services;
using FlawBOT.Services.Search;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class IMDBModule : BaseCommandModule
    {
        #region COMMAND_IMDB

        [Command("imdb")]
        [Aliases("omdb", "movie")]
        [Description("Retrieve a movie or TV show from OMDB")]
        public async Task OMDB(CommandContext ctx,
            [Description("Movie or TV show to find on IMDB")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = IMDBService.GetMovieDataAsync(query.Replace(" ", "+")).Result;
            if (results.Response == "False")
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
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
                    .AddField("IMDB Rating", results.IMDbRating, true)
                    .AddField("Metacritic", results.Metascore, true)
                    .AddField("Director", results.Director, true)
                    .AddField("Actors", results.Actors, true)
                    .WithColor(DiscordColor.Goldenrod);
                if (results.Poster != "N/A") output.WithImageUrl(results.Poster);
                if (results.TomatoURL != "N/A") output.WithUrl(results.TomatoURL);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_IMDB
    }
}