using FlawBOT.Services;
using NUnit.Framework;

namespace Modules
{
    [TestFixture]
    internal class News
    {
        [Test]
        public void GetNewsData()
        {
            Assert.IsTrue(NewsService.GetNewsDataAsync().Result.Status == "ok");
            Assert.IsTrue(NewsService.GetNewsDataAsync("Nintendo").Result.Status == "ok");
        }
    }
}