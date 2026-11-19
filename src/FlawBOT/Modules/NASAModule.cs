using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class NasaModule : ApplicationCommandModule
    {
        [SlashCommand("nasa", "Returns the Picture of the Day from NASA.")]
        public async Task GetNasaImage(InteractionContext ctx)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await NasaService.GetNasaImageAsync(Program.Settings.Tokens.NasaToken).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.ERR_API_CONNECTION, ResponseType.Warning).ConfigureAwait(false);
        }
    }
}
