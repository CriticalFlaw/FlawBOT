using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace GamesModule
{
    [TestFixture]
    internal class SpeedrunTests
    {
        [Test]
        public void GetSpeedrunGame()
        {
            Assert.Greater(SpeedrunService.GetSpeedrunGameAsync("Wind Waker").Result.Data.Count, 0);
            Assert.AreEqual(SpeedrunService.GetSpeedrunGameAsync("Wind Wanker").Result.Data.Count, 0);
        }
    }
}