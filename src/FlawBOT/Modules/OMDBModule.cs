using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class OmdbModule : ApplicationCommandModule
    {
        [SlashCommand("omdb", "Returns a movie or TV show from OMDB.")]
        public async Task GetOMDBData(InteractionContext ctx, [Option("search", "Movie or TV show to find on OMDB.")] string query)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await OmdbService.GetOMDBDataAsync(Program.Settings.Tokens.OmdbToken, query).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
        }
    }
}
