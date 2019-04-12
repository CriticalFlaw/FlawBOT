using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Services;
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
            var rnd = new Random();
            var imgur = new ImgurClient(GlobalVariables.config.ImgurToken);
            var endpoint = new GalleryEndpoint(imgur);
            var gallery = string.IsNullOrWhiteSpace(query) ? (await endpoint.GetRandomGalleryAsync()).ToList() : (await endpoint.SearchGalleryAsync(query)).ToList();
            var img = gallery.Any() ? gallery[rnd.Next(0, gallery.Count)] : null;
            var output = new DiscordEmbedBuilder();
            switch (img)
            {
                case GalleryAlbum _:
                    output.WithImageUrl(((GalleryAlbum)img).Link);
                    output.WithColor(new DiscordColor("#85BF25"));
                    break;

                case GalleryImage _:
                    output.WithImageUrl(((GalleryImage)img).Link);
                    output.WithColor(new DiscordColor("#85BF25"));
                    break;

                default:
                    output.WithTitle(":mag: No results found!");
                    output.WithColor(DiscordColor.Yellow);
                    break;
            }
            await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_IMGUR
    }
}