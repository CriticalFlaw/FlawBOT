using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class DictionaryModule : ApplicationCommandModule
    {
        [SlashCommand("dictionary", "Retrieve an Urban Dictionary definition for a word or phrase.")]
        public async Task UrbanDictionary(InteractionContext ctx, [Option("search", "Word or phrase to find on Urban Dictionary.")] string query = "")
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