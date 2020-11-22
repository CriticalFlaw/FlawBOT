using System.Threading.Tasks;
using FlawBOT.Services;
using NUnit.Framework;

namespace Modules
{
    [TestFixture]
    internal class Imgur
    {
        [Test]
        public async Task GetImgurGalleryData()
        {
            var results = await ImgurService.GetImgurGalleryAsync("cats").ConfigureAwait(false);
            Assert.IsNotNull(results);

            results = await ImgurService.GetImgurGalleryAsync("dogs").ConfigureAwait(false);
            Assert.IsNotNull(results);
        }
    }
}