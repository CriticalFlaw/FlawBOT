using DSharpPlus.SlashCommands;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class SimpsonsModule : ApplicationCommandModule
    {
        [SlashCommand("simpsons", "Retrieve a random Simpsons screenshot and episode.")]
        public async Task Simpsons(InteractionContext ctx)
        {
            var result = await SimpsonsService.GetEpisodeGifAsync(SimpsonsService.SiteRoot.Frinkiac).ConfigureAwait(false);
            await ctx.CreateResponseAsync(result.Build()).ConfigureAwait(false);
        }

        [SlashCommand("futurama", "Retrieve a random Futurama screenshot and episode.")]
        public async Task Futurama(InteractionContext ctx)
        {
            var result = await SimpsonsService.GetEpisodeGifAsync(SimpsonsService.SiteRoot.Morbotron).ConfigureAwait(false);
            await ctx.CreateResponseAsync(result.Build()).ConfigureAwait(false);
        }
    }
}