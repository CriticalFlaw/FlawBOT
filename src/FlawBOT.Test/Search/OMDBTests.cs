using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class OMDBTests
    {
        [Test]
        public void GetMovieData()
        {
            Assert.IsNotNull(OMDBService.GetMovieDataAsync("office+space").Result);
        }
    }
}