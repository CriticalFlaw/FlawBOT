using FlawBOT.Services;
using NUnit.Framework;
using System.Threading.Tasks;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Imgur
{
    [Test]
    public async Task GetImgurGalleryData()
    {
        var results = await ImgurService.GetImgurResultAsync(TestSetup.Tokens.ImgurToken, "cats").ConfigureAwait(false);
        Assert.IsNotNull(results);

        results = await ImgurService.GetImgurResultAsync(TestSetup.Tokens.ImgurToken, "dogs").ConfigureAwait(false);
        Assert.IsNotNull(results);
    }
}