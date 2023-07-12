using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class ImgurModule : ApplicationCommandModule
    {
        [SlashCommand("imgur", "Returns a queried image from Imgur.")]
        public async Task GetImgurResult(InteractionContext ctx, [Option("query", "Search query to pass to Imgur.")] string query)
        {
            var output = ImgurService.GetImgurResultAsync(Program.Settings.Tokens.ImgurToken, query).Result;
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_API_CONNECTION, ResponseType.Warning).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}