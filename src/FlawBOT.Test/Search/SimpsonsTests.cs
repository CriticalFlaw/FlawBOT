using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class SimpsonsTests
    {
        [Test]
        public void GetFuturamaEpisode()
        {
            Assert.NotNull(SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.Morbotron).Result.Title);
        }

        [Test]
        public void GetRickMortyEpisode()
        {
            Assert.NotNull(SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.MasterOfAllScience).Result.Title);
        }

        [Test]
        public void GetSimpsonsEpisode()
        {
            Assert.NotNull(SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.Frinkiac).Result.Title);
        }
    }
}