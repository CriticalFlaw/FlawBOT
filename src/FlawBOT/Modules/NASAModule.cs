using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class NasaModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns a Picture of the Day from NASA.
        /// </summary>
        [SlashCommand("nasa", "Returns a Picture of the Day from NASA.")]
        public async Task GetNasaImage(InteractionContext ctx)
        {
            var output = await NasaService.GetNasaImageAsync(Program.Settings.Tokens.NasaToken).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_API_CONNECTION, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}