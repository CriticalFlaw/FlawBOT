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
            Assert.IsTrue(GoodReadService.GetBookDataAsync("Ender's Game").Result.Books.Count > 0);
            Assert.IsFalse(GoodReadService.GetBookDataAsync("Bender's Game").Result.Books.Count > 0);
        }
    }
}