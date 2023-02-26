using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Modules.Bot;
using FlawBOT.Properties;
using Imgur.API.Models.Impl;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Images
{
    public class ImgurModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns an image or album from the Imgur API.
        /// </summary>
        [SlashCommand("imgur", "Retrieve an image from Imgur.")]
        public async Task Imgur(InteractionContext ctx, [Option("search", "Search query to pass to Imgur.")] string search)
        {
            var output = new DiscordEmbedBuilder().WithColor(new DiscordColor("#89C623"));
            switch (ImgurService.GetImgurGalleryAsync(Program.Settings.Tokens.ImgurToken, search).Result)
            {
                case GalleryImage image:
                    output.WithDescription(image.Title ?? "Search results for " + Formatter.Bold(search) + " on Imgur");
                    output.WithImageUrl(image.Link);
                    await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
                    break;

                case GalleryAlbum album:
                    output.WithDescription(album.Title ?? "Search results for " + Formatter.Bold(search) + " on Imgur");
                    output.WithImageUrl(album.Link);
                    await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
                    break;

                default:
                    await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                    break;
            }
        }
    }
}