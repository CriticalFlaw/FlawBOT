using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using FlawBOT.Services.Lookup;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
{
    public class NasaModule : ApplicationCommandModule
    {
        #region COMMAND_NASA

        [SlashCommand("nasa", "Retrieve NASA's Astronomy Picture of the Day.")]
        public async Task Nasa(InteractionContext ctx)
        {
            var results = await NasaService.GetNasaImageAsync(Program.Settings.Tokens.NasaToken).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_API_CONNECTION, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            var output = new DiscordEmbedBuilder()
                .WithDescription(results.Title)
                .WithImageUrl(results.ImageHd ?? results.ImageSd)
                .WithFooter(results.Description)
                .WithColor(new DiscordColor("#0B3D91"));
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_NASA
    }
}