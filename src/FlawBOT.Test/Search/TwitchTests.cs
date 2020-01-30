using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class TwitchTests
    {
        [Test]
        public void GetStreamData()
        {
            Assert.IsNotNull(TwitchService.GetTwitchDataAsync("rifftrax"));
        }
    }
}