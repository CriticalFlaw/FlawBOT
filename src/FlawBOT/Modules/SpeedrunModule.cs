using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class SpeedrunModule : ApplicationCommandModule
    {
        [SlashCommand("speedrun", "Returns game information from Speedrun.com")]
        public async Task GetSpeedrunGame(InteractionContext ctx, [Option("query", "Game to find on Speedrun.com")] string query)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await SpeedrunService.GetSpeedrunGameAsync(query).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
        }
    }
}
