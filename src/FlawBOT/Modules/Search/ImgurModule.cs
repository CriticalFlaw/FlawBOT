using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using FlawBOT.Common;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models.Impl;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
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
            var random = new Random();
            var imgur = new ImgurClient(SharedData.Tokens.ImgurToken);
            var endpoint = new GalleryEndpoint(imgur);
            var gallery = string.IsNullOrWhiteSpace(query) ? (await endpoint.GetRandomGalleryAsync()).ToList() : (await endpoint.SearchGalleryAsync(query)).ToList();
            var img = gallery.Any() ? gallery[random.Next(0, gallery.Count)] : null;
            switch (img)
            {
                case GalleryAlbum _:
                    await ctx.RespondAsync(((GalleryAlbum)img).Link);
                    break;

                case GalleryImage _:
                    await ctx.RespondAsync(((GalleryImage)img).Link);
                    break;

                default:
                    await ctx.RespondAsync(":mag: No results found!");
                    break;
            }
        }

        #endregion COMMAND_IMGUR
    }
}