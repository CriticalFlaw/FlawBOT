using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class NewsModule : ApplicationCommandModule
    {
        [SlashCommand("news", "Retrieve the news articles on a topic from NewsAPI.org.")]
        public async Task News(InteractionContext ctx, [Option("search", "Article topic to find on NewsAPI.org.")] string query)
        {
            var output = await NewsService.GetNewsDataAsync(Program.Settings.Tokens.NewsToken, query).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}