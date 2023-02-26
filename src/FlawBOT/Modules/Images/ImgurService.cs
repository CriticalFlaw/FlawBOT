using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Enums;
using Imgur.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Images
{
    public static class ImgurService
    {
        public static async Task<IGalleryItem> GetImgurGalleryAsync(string token, string query, GallerySortOrder order = GallerySortOrder.Top, TimeWindow time = TimeWindow.All)
        {
            var imgur = new GalleryEndpoint(new ImgurClient(token));
            var result = string.IsNullOrWhiteSpace(query)
                ? (await imgur.GetRandomGalleryAsync().ConfigureAwait(false)).ToList()
                : (await imgur.SearchGalleryAsync(query, order, time).ConfigureAwait(false)).ToList();
            return result.Count > 0 ? result[new Random().Next(0, result.Count)] : null;
        }
    }
}