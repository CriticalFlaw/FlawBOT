using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class WikipediaModule : BaseCommandModule
    {
        #region COMMAND_WIKIPEDIA

        [Command("wiki")]
        [Aliases("wikipedia")]
        [Description("Search Wikipedia for a given query")]
        public async Task Wikipedia(CommandContext ctx,
            [Description("Query to search on Wikipedia")] [RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = WikipediaService.GetWikipediaDataAsync(query);
            if (results.Error != null || results.Search.Count == 0)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_WIKIPEDIA, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            var output = new DiscordEmbedBuilder()
                .WithFooter("Articles retrieved using WikipediaNET")
                .WithColor(new DiscordColor("#E7B53B"));

            foreach (var result in results.Search)
            {
                var desc = Regex.Replace(
                    result.Snippet.Length <= 300
                        ? result.Snippet
                        : result.Snippet.Substring(0, 150) + "...", "<[^>]*>", "") ?? "Article has not content.";

                output.AddField(result.Title, $"[[Link]({result.Url.AbsoluteUri})] {desc}");
            }

            await ctx.RespondAsync("Search results for " + Formatter.Bold(query) + " on Wikipedia", output)
                .ConfigureAwait(false);
        }

        #endregion COMMAND_WIKIPEDIA
    }
}