using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using FlawBOT.Framework.Services.Search;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class GoodReadModule : BaseCommandModule
    {
        #region COMMAND_BOOKS

        [Command("book")]
        [Aliases("goodreads", "books")]
        [Description("Retrieve a book from GoodReads")]
        public async Task Books(CommandContext ctx,
            [Description("Book title to find on GoodReads")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = GoodReadService.GetBookDataAsync(query).Result;
            if (results.Books.Count > 0)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing);
            else
            {
                foreach (var book in results.Books)
                {
                    // TODO: Add page count, publication, ISBN, URLs
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(book.Work.Title)
                        .AddField("Author", book.Work.Author.Name ?? "Unknown", true)
                        .AddField("Publication Year", book.PublicationYear.Text ?? "Unknown", true)
                        .AddField("Ratings", $"Average {book.RatingAverage} ({book.RatingCount} total ratings)", true)
                        .WithImageUrl(book.Work.ImageUrl ?? book.Work.ImageUrlSmall)
                        .WithFooter("Type 'next' within 10 seconds for the next book.")
                        .WithColor(new DiscordColor("#372213"));
                    var message = await ctx.RespondAsync(embed: output.Build());

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
                    if (interactivity.Result == null) break;
                    await BotServices.RemoveMessage(interactivity.Result);
                    await BotServices.RemoveMessage(message);
                }
            }
        }

        #endregion COMMAND_BOOKS
    }
}