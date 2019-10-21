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
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing);
            else
            {
                foreach (var book in results.Results)
                {
                    // TODO: Add page count, publication, ISBN, URLs
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(book.Book.Title)
                        .AddField("Author", book.Book.Author.Name ?? "Unknown", true)
                        .AddField("Publication Year", book.PublicationYear.Text ?? "Unknown", true)
                        .AddField("Ratings", $"Average {book.RatingAverage} ({book.RatingCount.Text} total ratings)", true)
                        .WithImageUrl(book.Book.ImageUrl ?? book.Book.ImageUrlSmall)
                        .WithFooter("Type 'next' within 10 seconds for the next book.")
                        .WithColor(new DiscordColor("#372213"));
                    var message = await ctx.RespondAsync(embed: output.Build());

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
                    if (interactivity.Result is null) break;
                    await BotServices.RemoveMessage(interactivity.Result);
                }
            }
        }

        #endregion COMMAND_BOOKS
    }
}