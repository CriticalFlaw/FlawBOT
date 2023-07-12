using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class NintendoModule : ApplicationCommandModule
    {
        [SlashCommand("amiibo", "Returns information about an Amiibo figurine or card.")]
        public async Task GetAmiiboInfo(InteractionContext ctx, [Option("query", "Name of an Amiibo figurine.")] string query)
        {
            var output = await NintendoService.GetAmiiboInfoAsync(query).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        [SlashCommand("pokemon", "Returns a Pokémon card.")]
        public async Task GetPokemonCard(InteractionContext ctx, [Option("query", "Name of a Pokémon.")] string query = "")
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