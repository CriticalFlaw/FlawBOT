using FlawBOT.Framework.Services;
using Imgur.API.Enums;
using NUnit.Framework;
using System.Threading.Tasks;

namespace SearchModule
{
    [TestFixture]
    internal class ImgurTests
    {
        [Test, Ignore("API requires key.")]
        public async Task GetImgurGalleryData()
        {
            var results = await ImgurService.GetImgurGalleryAsync("cats", GallerySortOrder.Top, TimeWindow.All).ConfigureAwait(false);
            Assert.IsNotNull(results);

            results = await ImgurService.GetImgurGalleryAsync("dogs", GallerySortOrder.Top, TimeWindow.All).ConfigureAwait(false);
            Assert.IsNotNull(results);
        }
    }
}