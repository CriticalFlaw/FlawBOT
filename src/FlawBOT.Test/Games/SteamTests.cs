using System.Text.RegularExpressions;
using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace GamesModule
{
    [TestFixture]
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

        [Test]
        public void GetConnectLink()
        {
            var regex = new Regex(@"\s*(?'ip'\S+)\s*", RegexOptions.Compiled).Match("192.168.22.11");
            Assert.IsTrue(regex.Success);
        }
    }
}