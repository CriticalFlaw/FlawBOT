using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace Modules
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