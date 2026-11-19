using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class NewsModule : ApplicationCommandModule
    {
        [SlashCommand("news", "Returns articles from NewsAPI.")]
        public async Task GetNewsArticles(InteractionContext ctx, [Option("query", "Topic to search on NewsAPI.org.")] string query)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await NewsService.GetNewsArticlesAsync(Program.Settings.Tokens.NewsToken, query).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
        }
    }
}
