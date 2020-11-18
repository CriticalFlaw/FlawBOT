using System.Text.RegularExpressions;
using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace Modules
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
            Assert.IsNotNull(SteamService.GetSteamProfileAsync("criticalflaw").Result);
            Assert.IsNull(SteamService.GetSteamProfileAsync("99999999999999999").Result);
        }

        [Test]
        [Order(1)]
        public void UpdateSteamAppList()
        {
            Assert.IsTrue(SteamService.UpdateSteamAppListAsync().Result);
        }
    }
}