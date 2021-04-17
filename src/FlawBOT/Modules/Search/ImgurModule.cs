using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using Imgur.API.Models.Impl;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class ImgurModule : BaseCommandModule
    {
        #region COMMAND_IMGUR

        [Command("imgur")]
        [Aliases("image")]
        [Description("Retrieve an image from Imgur")]
        public async Task Imgur(CommandContext ctx,
            [Description("Search query to pass to Imgur")] [RemainingText]
            string query)
        {
            await ctx.TriggerTypingAsync();
            var results = ImgurService.GetImgurGalleryAsync(query).Result;
            var output = new DiscordEmbedBuilder().WithColor(new DiscordColor("#89C623"));

            switch (results)
            {
                case GalleryImage image:
                    output.WithDescription(image.Title ?? "Search results for " + Formatter.Bold(query) + " on Imgur");
                    output.WithImageUrl(image.Link);
                    await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
                    break;

                case GalleryAlbum album:
                    output.WithDescription(album.Title ?? "Search results for " + Formatter.Bold(query) + " on Imgur");
                    output.WithImageUrl(album.Link);
                    await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
                    break;

                default:
                    await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                        .ConfigureAwait(false);
                    break;
            }
        }

        #endregion COMMAND_IMGUR
    }
}