using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    internal class TwitchTests
    {
        [Test]
        public void GetStreamData()
        {
            Assert.NotNull(TwitchService.GetTwitchDataAsync("rifftrax"));
        }
    }
}