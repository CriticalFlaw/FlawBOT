using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test
{
    [TestFixture]
    internal class News
    {
        [Test]
        public void GetNewsData()
        {
            Assert.IsTrue(NewsService.GetNewsDataAsync(TestSetup.Tokens.NewsToken).Result.Status == "ok");
            Assert.IsTrue(NewsService.GetNewsDataAsync(TestSetup.Tokens.NewsToken, "Nintendo").Result.Status ==
                          "ok");
        }
    }
}