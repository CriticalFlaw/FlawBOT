using FlawBOT.Framework.Services;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace GamesModule
{
    [TestFixture]
    internal class SteamTests
    {
        [Test]
        public void GenerateConnectionLink()
        {
            var regex = new Regex(@"\s*(?'ip'\S+)\s*", RegexOptions.Compiled).Match("192.168.22.11");
            Assert.IsTrue(regex.Success);
        }

        [Test]
        public void GetSteamProfile()
        {
            Assert.IsNotNull(SteamService.GetSteamProfileAsync("criticalflaw").Result);
            Assert.IsNotNull(SteamService.GetSteamSummaryAsync("criticalflaw").Result);
            Assert.IsNull(SteamService.GetSteamProfileAsync("99999999999999999").Result);
            Assert.IsNull(SteamService.GetSteamSummaryAsync("99999999999999999").Result);
        }

        [Test]
        public void UpdateSteamList()
        {
            Assert.IsTrue(SteamService.UpdateSteamListAsync().Result);
        }
    }
}