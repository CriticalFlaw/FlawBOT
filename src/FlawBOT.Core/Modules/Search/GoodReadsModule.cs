using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using FlawBOT.Framework.Services.Search;
using System;
using System.Linq;
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
                        .AddField("Written by", book.Book.Author.Name ?? "Unknown", true)
                        .AddField("Publication Date", GoodReadsService.GetPublicationDate(book), true)
                        .AddField("Avg. Rating", book.RatingAverage ?? "Unknown", true)
                        .WithThumbnailUrl(book.Book.ImageUrl ?? book.Book.ImageUrlSmall)
                        .WithFooter((results.Results.Count > 1) ? "Type 'next' within 10 seconds for the next book." : "This is the only book of this name")
                        .WithColor(new DiscordColor("#372213"));
                    var message = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && string.Equals(m.Content, "next", StringComparison.InvariantCultureIgnoreCase), TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                    if (interactivity.Result is null) break;
                    await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                    if (!book.Equals(results.Results.Last()))
                        await BotServices.RemoveMessage(message).ConfigureAwait(false);
                }
            }
        }

        #endregion COMMAND_BOOKS
    }
}