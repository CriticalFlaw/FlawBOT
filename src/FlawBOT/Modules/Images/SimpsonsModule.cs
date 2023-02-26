using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Images
{
    public class SimpsonsModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns a Simpsons GIF and episode from the Frinkiac API.
        /// </summary>
        [SlashCommand("simpsons", "Retrieve a random Simpsons screenshot and episode.")]
        public async Task Simpsons(InteractionContext ctx)
        {
            var result = await SimpsonsService.GetEpisodeGifAsync(SimpsonsService.SiteRoot.Frinkiac).ConfigureAwait(false);
            await ctx.CreateResponseAsync(result.Build()).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a Futurama GIF and episode from the Morbotron API.
        /// </summary>
        [SlashCommand("futurama", "Retrieve a random Futurama screenshot and episode.")]
        public async Task Futurama(InteractionContext ctx)
        {
            var result = await SimpsonsService.GetEpisodeGifAsync(SimpsonsService.SiteRoot.Morbotron).ConfigureAwait(false);
            await ctx.CreateResponseAsync(result.Build()).ConfigureAwait(false);
        }
    }
}