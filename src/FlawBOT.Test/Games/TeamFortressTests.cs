using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace GamesModule
{
    [TestFixture]
    internal class TeamFortressTests
    {
        [Test]
        public void GetMapStats()
        {
            Assert.IsNotNull(TeamFortressService.GetMapStatsAsync("pl_upward").Result);
            Assert.IsNotNull(TeamFortressService.GetMapStatsAsync("upward").Result);
            Assert.IsNull(TeamFortressService.GetMapStatsAsync("onward").Result);
        }

        [Test]
        public void GetNewsOverview()
        {
            Assert.IsNotNull(TeamFortressService.GetNewsOverviewAsync().Result);
        }

        [Test]
        [Order(2)]
        public void GetSchemaItem()
        {
            Assert.IsNotNull(TeamFortressService.GetSchemaItem("scattergun"));
            Assert.IsNull(TeamFortressService.GetSchemaItem("shattergun"));
        }

        [Test]
        public void GetServers()
        {
            Assert.IsNotNull(TeamFortressService.GetGameModeServerAsync("payload").Result);
            Assert.IsNull(TeamFortressService.GetGameModeServerAsync("payloader").Result);
        }

        [Test]
        [Order(1)]
        public void UpdateTf2Schema()
        {
            Assert.IsTrue(TeamFortressService.UpdateTf2SchemaAsync().Result);
        }
    }
}