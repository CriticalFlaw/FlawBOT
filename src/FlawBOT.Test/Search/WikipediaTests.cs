using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class WikipediaTests
    {
        [Test]
        public void GetWikipediaPage()
        {
            Assert.IsFalse(WikipediaService.GetWikipediaDataAsync("Russia").Result.Missing);
        }
    }
}