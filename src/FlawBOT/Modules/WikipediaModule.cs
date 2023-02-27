using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class WikipediaModule : ApplicationCommandModule
    {
        [SlashCommand("wiki", "Find articles on Wikipedia.")]
        public async Task Wikipedia(InteractionContext ctx, [Option("query", "Articles to find on Wikipedia.")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await WikipediaService.GetWikipediaDataAsync(query).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_WIKIPEDIA, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            // TODO: Add pagination when supported for slash commands.
            var output = new DiscordEmbedBuilder()
                .WithTitle("Relevant articles found on Wikipedia.")
                .WithColor(new DiscordColor("#6B6B6B"));

            foreach (var result in results)
            {
                var desc = Regex.Replace(result.Snippet.Length <= 300
                    ? string.IsNullOrEmpty(result.Snippet) ? "Article has not content." : result.Snippet
                    : result.Snippet[..150] + "...", "<[^>]*>", "");
                output.AddField(result.Title, $"[[Link]({result.Url.AbsoluteUri})] {desc}");
            }
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }
    }
}