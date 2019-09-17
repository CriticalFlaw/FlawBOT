using FlawBOT.Framework.Services.Search;
using NUnit.Framework;

namespace FlawBOT.Test.Search
{
    [TestFixture]
    internal class GoodReadTests
    {
        [Test]
        public void GetBookData()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(GoodReadService.GetBookDataAsync("Ender's Game").Result.Title));
            Assert.IsFalse(string.IsNullOrWhiteSpace(GoodReadService.GetBookDataAsync("Bender's Game").Result.Title));
        }
    }
}