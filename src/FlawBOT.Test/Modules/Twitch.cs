using FlawBOT.Services;
using NUnit.Framework;

namespace Modules
{
    [TestFixture]
    internal class Twitch
    {
        [Test]
        public void GetStreamData()
        {
            Assert.IsNotNull(TwitchService.GetTwitchDataAsync("rifftrax"));
        }
    }
}