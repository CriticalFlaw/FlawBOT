using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    internal class OMDBTests
    {
        [Test]
        public void GetMovieData()
        {
            Assert.Null(OMDBService.GetMovieDataAsync("Office+Space").Result);
        }
    }
}