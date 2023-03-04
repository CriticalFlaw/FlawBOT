using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class NintendoModule : ApplicationCommandModule
    {
        [SlashCommand("amiibo", "Retrieve information about an Amiibo figurine.")]
        public async Task GetAmiibo(InteractionContext ctx, [Option("search", "Name of the Amiibo figurine.")] string query)
        {
            var output = await NintendoService.GetAmiiboDataAsync(query).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        [SlashCommand("pokemon", "Retrieve a Pokémon card.")]
        public async Task Pokemon(InteractionContext ctx, [Option("search", "Name of the Pokémon.")] string query = "")
        {
            var output = await NintendoService.GetPokemonCardAsync(Program.Settings.Tokens.PokemonToken, query).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}