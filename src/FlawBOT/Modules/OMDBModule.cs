using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class OmdbModule : ApplicationCommandModule
    {
        [SlashCommand("omdb", "Find a movie or TV show from OMDB.")]
        public async Task Omdb(InteractionContext ctx, [Option("search", "Movie or TV show to find on OMDB.")] string query)
        {
            var output = OmdbService.GetOMDBResult(Program.Settings.Tokens.OmdbToken, query).Result;
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}