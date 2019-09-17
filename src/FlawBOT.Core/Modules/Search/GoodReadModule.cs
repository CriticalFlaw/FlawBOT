using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using FlawBOT.Framework.Services.Search;

namespace FlawBOT.Core.Modules.Search
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class GoodReadModule : BaseCommandModule
    {
        #region COMMAND_OMDB

        [Command("book")]
        [Aliases("goodreads", "books")]
        [Description("Retrieve a book from GoodReads")]
        public async Task Books(CommandContext ctx,
            [Description("Book title to find on GoodReads")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = GoodReadService.GetBookDataAsync(query).Result;
            if (string.IsNullOrWhiteSpace(results.Title))
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.Title + $" ({results.Pages} pages)")
                    .WithDescription(results.Description.Length < 500 ? results.Description : results.Description.Take(500) + "...")
                    .AddField("Publish Date", results.PublicationDate.ToString() ?? "Unknown", true)
                    .AddField("Ratings", $"Average {results.AverageRating} ({results.RatingsCount} total ratings)", true)
                    .AddField("Publisher", results.Publisher, true)
                    .WithImageUrl(results.ImageUrl ?? results.SmallImageUrl)
                    .WithUrl(results.Url)
                    .WithFooter("ISBN: " + results.Isbn)
                    .WithColor(new DiscordColor("#372213"));

                var values = new StringBuilder();
                foreach (var author in results.Authors)
                    values.Append(author.Name + "\n");
                output.AddField("Author(s)", values.ToString() ?? "Unknown", true);

                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_OMDB
    }
}