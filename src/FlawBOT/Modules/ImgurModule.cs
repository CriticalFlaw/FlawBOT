using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class ImgurModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns an image or album from the Imgur API.
        /// </summary>
        [SlashCommand("imgur", "Retrieve an image from Imgur.")]
        public async Task Imgur(InteractionContext ctx, [Option("search", "Search query to pass to Imgur.")] string search)
        {
            var output = ImgurService.GetImgurGalleryAsync(Program.Settings.Tokens.ImgurToken, search).Result;
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_API_CONNECTION, ResponseType.Warning).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}