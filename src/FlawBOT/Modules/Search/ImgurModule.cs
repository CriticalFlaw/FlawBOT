using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using FlawBOT.Services.Lookup;
using Imgur.API.Models.Impl;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
{
    public class ImgurModule : ApplicationCommandModule
    {
        #region COMMAND_IMGUR

        [SlashCommand("imgur", "Retrieve an image from Imgur.")]
        public async Task Imgur(InteractionContext ctx, [Option("query", "Search query to pass to Imgur.")] string query)
        {
            var results = ImgurService.GetImgurGalleryAsync(Program.Settings.Tokens.ImgurToken, query).Result;
            var output = new DiscordEmbedBuilder().WithColor(new DiscordColor("#89C623"));

            switch (results)
            {
                case GalleryImage image:
                    output.WithDescription(image.Title ?? "Search results for " + Formatter.Bold(query) + " on Imgur");
                    output.WithImageUrl(image.Link);
                    await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
                    break;

                case GalleryAlbum album:
                    output.WithDescription(album.Title ?? "Search results for " + Formatter.Bold(query) + " on Imgur");
                    output.WithImageUrl(album.Link);
                    await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
                    break;

                default:
                    await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                    break;
            }
        }

        #endregion COMMAND_IMGUR
    }
}