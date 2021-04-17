using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test
{
    [TestFixture]
    internal class Twitch
    {
        [Test]
        public void GetStreamData()
        {
            Assert.IsNotNull(TwitchService.GetTwitchDataAsync(TestSetup.Tokens.TwitchToken, "rifftrax"));
        }
    }
}