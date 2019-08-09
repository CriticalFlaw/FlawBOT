using System;
using System.Linq;
using System.Threading.Tasks;
using FlawBOT.Framework.Common;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Enums;
using Imgur.API.Models;

namespace FlawBOT.Framework.Services
{
    public class ImgurService
    {
        public static async Task<IGalleryItem> GetImgurGalleryAsync(string query, GallerySortOrder order = GallerySortOrder.Top, TimeWindow time = TimeWindow.All)
        {
            var random = new Random();
            var imgur = new ImgurClient(TokenHandler.Tokens.ImgurToken);
            var endpoint = new GalleryEndpoint(imgur);
            var gallery = string.IsNullOrWhiteSpace(query) ? (await endpoint.GetRandomGalleryAsync()).ToList() : (await endpoint.SearchGalleryAsync(query, order, time)).ToList();
            return gallery.Any() ? gallery[random.Next(0, gallery.Count)] : null;
        }
    }
}