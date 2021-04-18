using System.Text.RegularExpressions;
using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test
{
    [TestFixture]
    internal class Steam
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
            Assert.IsNotNull(SteamService.GetSteamProfileAsync(TestSetup.Tokens.SteamToken, "criticalflaw")
                .Result);
            Assert.IsNull(SteamService.GetSteamProfileAsync(TestSetup.Tokens.SteamToken, "99999999999999999")
                .Result);
        }

        [Test]
        [Order(1)]
        public void UpdateSteamAppList()
        {
            Assert.IsTrue(SteamService.UpdateSteamAppListAsync(TestSetup.Tokens.SteamToken).Result);
        }
    }
}