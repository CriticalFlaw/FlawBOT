using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class OMDBTests
    {
        [Test, Ignore("API requires key.")]
        public void GetMovieData()
        {
            Assert.IsNotNull(OMDBService.GetMovieDataAsync("office+space").Result);
        }
    }
}