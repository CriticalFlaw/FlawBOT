using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class WikipediaModule : ApplicationCommandModule
    {
        [SlashCommand("wikipedia", "Find articles on Wikipedia.")]
        public async Task Wikipedia(InteractionContext ctx, [Option("query", "Articles to find on Wikipedia.")] string query)
        {
            var output = await WikipediaService.GetWikipediaDataAsync(query).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_WIKIPEDIA, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}