using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using FlawBOT.Services.Lookup;

namespace FlawBOT.Modules.Search
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class WikipediaModule : BaseCommandModule
    {
        #region COMMAND_WIKIPEDIA

        [Command("wiki")]
        [Aliases("wikipedia")]
        [Description("Find articles on Wikipedia.")]
        public async Task Wikipedia(CommandContext ctx,
            [Description("Articles to find on Wikipedia.")] [RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            await ctx.TriggerTypingAsync();
            var results = WikipediaService.GetWikipediaDataAsync(query);
            if (results.Error != null || results.Search?.Count == 0)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_WIKIPEDIA, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            if (results.Search?.Count <= 1)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            while (results.Search?.Count > 0)
            {
                var output = new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#6B6B6B"))
                    .WithFooter(results.Search.Count - 5 >= 5
                        ? "Type 'next' within 10 seconds for the next five articles."
                        : "There articles are retrieved using WikipediaNET.");

                foreach (var result in results.Search.Take(5))
                {
                    var desc = Regex.Replace(
                        result.Snippet.Length <= 300
                            ? string.IsNullOrEmpty(result.Snippet) ? "Article has not content." : result.Snippet
                            : result.Snippet[..150] + "...", "<[^>]*>", "");
                    output.AddField(result.Title, $"[[Link]({result.Url.AbsoluteUri})] {desc}");

                    results.Search.Remove(result);
                }

                var message = await ctx
                    .RespondAsync("Search results for " + Formatter.Bold(query) + " on Wikipedia", output)
                    .ConfigureAwait(false);

                if (results.Search.Count < 5) break;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                await BotServices.RemoveMessage(message).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_WIKIPEDIA
    }
}