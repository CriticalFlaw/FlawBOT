using FlawBOT.Framework.Services.Search;
using NUnit.Framework;

namespace FlawBOT.Test.Search
{
    [TestFixture]
    internal class GoodReadsTests
    {
        [Test]
        public void GetBookData()
        {
            Assert.IsTrue(GoodReadsService.GetBookDataAsync("Ender's Game").Result.Search.ResultCount > 0);
            Assert.IsFalse(GoodReadsService.GetBookDataAsync("Bender's Game").Result.Search.ResultCount > 0);
        }
    }
}