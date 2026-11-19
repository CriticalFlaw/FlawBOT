using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class WikipediaModule : ApplicationCommandModule
    {
        [SlashCommand("wikipedia", "Returns articles found on Wikipedia.")]
        public async Task GetWikipediaArticles(InteractionContext ctx, [Option("query", "Articles to find on Wikipedia.")] string query)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await WikipediaService.GetWikipediaArticlesAsync(query).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_WIKIPEDIA, ResponseType.Missing).ConfigureAwait(false);
        }
    }
}
