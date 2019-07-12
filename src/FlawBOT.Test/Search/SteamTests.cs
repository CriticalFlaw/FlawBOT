using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    internal class SteamTests
    {
        [Test]
        public void LoadAppList()
        {
            Assert.IsTrue(SteamService.UpdateSteamListAsync().Result);
        }

        [Test]
        public void GetUserData()
        {
            Assert.NotNull(SteamService.GetSteamUserProfileAsync("criticalflaw"));
            Assert.NotNull(SteamService.GetSteamUserSummaryAsync("criticalflaw"));
        }
    }
}