using System.Threading.Tasks;
using FlawBOT.Framework.Services;
using Imgur.API.Enums;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class ImgurTests
    {
        [Test]
        public async Task GetImgurGalleryData()
        {
            var results = await ImgurService.GetImgurGalleryAsync("cats", GallerySortOrder.Top, TimeWindow.All);
            Assert.IsNotNull(results);

            results = await ImgurService.GetImgurGalleryAsync("dogs", GallerySortOrder.Top, TimeWindow.All);
            Assert.IsNotNull(results);
        }
    }
}