using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class News
{
    [Test]
    public void GetNewsData()
    {
        Assert.IsNotNull(NewsService.GetNewsArticlesAsync(TestSetup.Tokens.NewsToken, "bitcoin"));
    }
}