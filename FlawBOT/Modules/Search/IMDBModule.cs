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
    public class IMDBModule : BaseCommandModule
    {
        #region COMMAND_IMDB

        [Command("imdb")]
        [Aliases("omdb")]
        [Description("Get a movie or TV show from OMDB")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task OMDB(CommandContext ctx, [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(ctx, query).Result) return;
            var data = IMDBService.GetMovieDataAsync(query).Result;
            if (data.Response == "False")
                await BotServices.SendEmbedAsync(ctx, ":mag: No results found!", EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(data.Title)
                    .WithDescription(data.Plot.Length < 500 ? data.Plot : data.Plot.Take(500) + "...")
                    .AddField("Released", data.Released, true)
                    .AddField("Runtime", data.Runtime, true)
                    .AddField("Genre", data.Genre, true)
                    .AddField("Country", data.Country, true)
                    .AddField("Box Office", data.BoxOffice, true)
                    .AddField("Production", data.Production, true)
                    .AddField("IMDB Rating", data.IMDbRating, true)
                    .AddField("Metacritic", data.Metascore, true)
                    .AddField("Rotten Tomatoes", data.TomatoRating, true)
                    .AddField("Director", data.Director, true)
                    .AddField("Actors", data.Actors, true)
                    .WithColor(DiscordColor.Goldenrod);
                if (data.Poster != "N/A") output.WithImageUrl(data.Poster);
                if (data.TomatoURL != "N/A") output.WithUrl(data.TomatoURL);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_IMDB
    }
}