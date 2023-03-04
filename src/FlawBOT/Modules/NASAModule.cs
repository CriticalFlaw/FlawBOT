using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class NasaModule : ApplicationCommandModule
    {
        [SlashCommand("nasa", "Retrieve NASA's Astronomy Picture of the Day.")]
        public async Task Nasa(InteractionContext ctx)
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