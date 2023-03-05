using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class DictionaryModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns a definition for a word of phrase from Urban Dictionary.
        /// </summary>
        [SlashCommand("definition", "Returns a definition for a word of phrase from Urban Dictionary.")]
        public async Task GetDictionaryDefinition(InteractionContext ctx, [Option("query", "Word or phrase to search on Urban Dictionary.")] string query = "")
        {
            var output = await DictionaryService.GetDictionaryDefinitionAsync(query).ConfigureAwait(false);
            if (output == null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}