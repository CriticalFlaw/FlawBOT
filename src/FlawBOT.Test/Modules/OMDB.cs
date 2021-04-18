using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test
{
    [TestFixture]
    internal class OMDB
    {
        [Test]
        public void GetMovieData()
        {
            Assert.IsNotNull(OmdbService.GetMovieDataAsync(TestSetup.Tokens.OmdbToken, "office+space").Result);
        }
    }
}