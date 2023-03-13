using DSharpPlus.Entities;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Enums;
using Imgur.API.Models.Impl;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public static class ImgurService
    {
        public static async Task<DiscordEmbed> GetImgurResultAsync(string token, string query, GallerySortOrder order = GallerySortOrder.Top, TimeWindow time = TimeWindow.All)
        {
            var imgur = new GalleryEndpoint(new ImgurClient(token));
            var result = string.IsNullOrWhiteSpace(query)
                ? (await imgur.GetRandomGalleryAsync().ConfigureAwait(false)).ToList()
                : (await imgur.SearchGalleryAsync(query, order, time).ConfigureAwait(false)).ToList();
            var results = result.Count > 0 ? result[new Random().Next(0, result.Count)] : null;

            var output = new DiscordEmbedBuilder().WithColor(new DiscordColor("#89C623"));
            switch (results)
            {
                case GalleryImage image:
                    output.WithImageUrl(image.Link);
                    output.WithFooter(image.Title ?? "Found via ImgurAPI");
                    break;

                case GalleryAlbum album:
                    output.WithImageUrl(album.Link);
                    output.WithFooter(album.Title ?? "Found via ImgurAPI");
                    break;

                default:
                    return null;
            }
            return output.Build();
        }
    }
}