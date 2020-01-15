using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using FlawBOT.Framework.Services.Search;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class GoodReadsModule : BaseCommandModule
    {
        #region COMMAND_BOOKS

        [Command("book")]
        [Aliases("goodreads", "books")]
        [Description("Retrieve book information from GoodReads")]
        public async Task Books(CommandContext ctx,
            [Description("Book title to find on GoodReads")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = GoodReadsService.GetBookDataAsync(query).Result.Search;
            if (results.ResultCount <= 0)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                foreach (var book in results.Results)
                {
                    // TODO: Add page count, publication, ISBN, URLs
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(book.Book.Title)
                        .AddField("Publication Year", GoodReadsService.GetPublicationDate(book).Result, true)
                        .AddField("Author", book.Book.Author.Name ?? "Unknown", true)
                        .AddField("Avg. Rating", book.RatingAverage ?? "Unknown")
                        .AddField("Total Ratings", book.RatingCount.Text ?? "Unknown")
                        .WithThumbnailUrl(book.Book.ImageUrl ?? book.Book.ImageUrlSmall)
                        .WithFooter("Type 'next' within 10 seconds for the next book.")
                        .WithColor(new DiscordColor("#372213"));
                    var message = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && string.Equals(m.Content, "next", StringComparison.InvariantCultureIgnoreCase), TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                    if (interactivity.Result is null) break;
                    await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                }
            }
        }

        #endregion COMMAND_BOOKS
    }
}