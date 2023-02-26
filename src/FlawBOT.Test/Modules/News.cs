using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class News
{
    [Test]
    public void GetNewsData()
    {
        Assert.IsTrue(NewsService.GetNewsDataAsync(TestSetup.Tokens.NewsToken, "bitcoin").Result.Status.Equals("ok"));
    }
}