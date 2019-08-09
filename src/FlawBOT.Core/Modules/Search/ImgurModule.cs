using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using FlawBOT.Framework.Services;
using Imgur.API.Models.Impl;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class ImgurModule : BaseCommandModule
    {
        #region COMMAND_IMGUR

        [Command("imgur")]
        [Description("Retrieve an imager from Imgur")]
        public async Task Imgur(CommandContext ctx,
            [Description("Search query to pass to Imgur")] [RemainingText] string query)
        {
            var results = ImgurService.GetImgurGalleryAsync(query).Result;
            switch (results)
            {
                case GalleryAlbum _:
                    await ctx.RespondAsync(((GalleryAlbum)results).Link);
                    break;

                case GalleryImage _:
                    await ctx.RespondAsync(((GalleryImage)results).Link);
                    break;

                default:
                    await ctx.RespondAsync(":mag: No results found!");
                    break;
            }
        }

        #endregion COMMAND_IMGUR
    }
}