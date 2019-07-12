using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace GamesModule
{
    internal class SpeedrunTests
    {
        [Test]
        public void GetSpeedrunGame()
        {
            Assert.IsNotNull(SpeedrunService.GetSpeedrunGameAsync("Wind Waker"));
        }
    }
}