using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class NewsModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns articles from NewsAPI.
        /// </summary>
        [SlashCommand("news", "Returns articles from NewsAPI.")]
        public async Task GetNewsArticles(InteractionContext ctx, [Option("query", "Topic to search on NewsAPI.org.")] string query)
        {
            var output = await NewsService.GetNewsArticlesAsync(Program.Settings.Tokens.NewsToken, query).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}