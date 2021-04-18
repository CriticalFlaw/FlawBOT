using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test
{
    [TestFixture]
    internal class Wikipedia
    {
        [Test]
        public void GetWikipediaPage()
        {
            Assert.IsNull(WikipediaService.GetWikipediaDataAsync("Russia").Error);
        }
    }
}