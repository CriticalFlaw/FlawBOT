using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;
using static FlawBOT.Services.SimpsonsService;

namespace FlawBOT.Modules
{
    public class SimpsonsModule : ApplicationCommandModule
    {
        [SlashCommand("simpsons", "Returns a random Simpsons episode GIF.")]
        public Task Simpsons(InteractionContext ctx)
        {
            return GetEpisodePost(ctx, SiteRoot.Frinkiac);
        }
        [SlashCommand("futurama", "Returns a random Futurama episode GIF.")]
        public Task Futurama(InteractionContext ctx)
        {
            return GetEpisodePost(ctx, SiteRoot.Morbotron);
        }

        private static async Task GetEpisodePost(InteractionContext ctx, SiteRoot show)
        {
            var output = await GetEpisodePostAsync(show).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}