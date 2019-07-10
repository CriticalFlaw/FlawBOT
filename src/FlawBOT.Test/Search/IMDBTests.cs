using FlawBOT.Services.Search;
using NUnit.Framework;

namespace SearchModule
{
    internal class IMDBTests
    {
        [Test]
        public void GetMovieData()
        {
            Assert.Null(IMDBService.GetMovieDataAsync("Office+Space").Result);
        }
    }
}