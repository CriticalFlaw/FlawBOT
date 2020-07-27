using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class OmdbTests
    {
        [Test]
        public void GetMovieData()
        {
            Assert.IsNotNull(OmdbService.GetMovieDataAsync("office+space").Result);
        }
    }
}