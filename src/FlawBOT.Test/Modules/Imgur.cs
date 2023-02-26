using FlawBOT.Modules.Images;
using NUnit.Framework;
using System.Threading.Tasks;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Imgur
{
    [Test]
    public async Task GetImgurGalleryData()
    {
        var results = await ImgurService.GetImgurGalleryAsync(TestSetup.Tokens.ImgurToken, "cats").ConfigureAwait(false);
        Assert.IsNotNull(results);

        results = await ImgurService.GetImgurGalleryAsync(TestSetup.Tokens.ImgurToken, "dogs").ConfigureAwait(false);
        Assert.IsNotNull(results);
    }
}