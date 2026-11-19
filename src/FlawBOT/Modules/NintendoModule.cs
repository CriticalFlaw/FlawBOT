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
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await NintendoService.GetAmiiboInfoAsync(query).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
        }

        [SlashCommand("pokemon", "Returns a Pokémon card.")]
        public async Task GetPokemonCard(InteractionContext ctx, [Option("query", "Name of a Pokémon.")] string query = "")
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await NintendoService.GetPokemonCardAsync(Program.Settings.Tokens.PokemonToken, query).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
        }
    }
}
