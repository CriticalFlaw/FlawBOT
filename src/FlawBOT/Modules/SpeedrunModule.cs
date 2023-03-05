using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class SpeedrunModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns game information from Speedrun.com
        /// </summary>
        [SlashCommand("speedrun-game", "Returns game information from Speedrun.com")]
        public async Task GetSpeedrunGame(InteractionContext ctx, [Option("query", "Game to find on Speedrun.com")] string query)
        {
            var output = SpeedrunService.GetSpeedrunGameAsync(query).Result;
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}