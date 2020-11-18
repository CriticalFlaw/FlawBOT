using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace Modules
{
    [TestFixture]
    internal class OMDB
    {
        [Test]
        public void GetMovieData()
        {
            Assert.IsNotNull(OmdbService.GetMovieDataAsync("office+space").Result);
        }
    }
}