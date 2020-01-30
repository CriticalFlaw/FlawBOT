using FlawBOT.Framework.Services.Search;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class GoodReadsTests
    {
        [Test, Ignore("API requires key.")]
        public void GetBookData()
        {
            Assert.Greater(GoodReadsService.GetBookDataAsync("Ender's Game").Result.Search.ResultCount, 0);
            Assert.AreEqual(GoodReadsService.GetBookDataAsync("Bender's Game").Result.Search.ResultCount, 0);
        }
    }
}