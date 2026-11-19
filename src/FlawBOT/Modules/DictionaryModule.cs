using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class DictionaryModule : ApplicationCommandModule
    {
        [SlashCommand("define", "Returns a definition for a word or phrase from Urban Dictionary.")]
        public async Task GetDictionaryDefinition(InteractionContext ctx, [Option("query", "Word or phrase to search on Urban Dictionary. Leave empty for a random word.")] string query = "")
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await DictionaryService.GetDictionaryDefinitionAsync(query).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
        }
    }
}
