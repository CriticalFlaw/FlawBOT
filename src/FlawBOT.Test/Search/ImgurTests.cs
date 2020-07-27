using System.Threading.Tasks;
using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class ImgurTests
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