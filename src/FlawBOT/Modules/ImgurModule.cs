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
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await ImgurService.GetImgurResultAsync(Program.Settings.Tokens.ImgurToken, query).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
        }
    }
}
