using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using Imgur.API.Models.Impl;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class ImgurModule : BaseCommandModule
    {
        #region COMMAND_IMGUR

        [Command("imgur")]
        [Aliases("image")]
        [Description("Retrieve an imager from Imgur")]
        public async Task Imgur(CommandContext ctx,
            [Description("Search query to pass to Imgur")] [RemainingText] string query)
        {
            var results = ImgurService.GetImgurGalleryAsync(query).Result;
            switch (results)
            {
                case GalleryAlbum _:
                    await ctx.RespondAsync(((GalleryAlbum)results).Link).ConfigureAwait(false);
                    break;

                case GalleryImage _:
                    await ctx.RespondAsync(((GalleryImage)results).Link).ConfigureAwait(false);
                    break;

                default:
                    await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
                    break;
            }
        }

        #endregion COMMAND_IMGUR
    }
}