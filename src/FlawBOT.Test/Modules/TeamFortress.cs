using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace Modules
{
    [TestFixture]
    internal class TeamFortress
    {
        [Test]
        public void GetMapStats()
        {
            Assert.IsNotNull(TeamFortressService.GetMapStatsAsync("pl_upward").Result);
            Assert.IsNotNull(TeamFortressService.GetMapStatsAsync("upward").Result);
            Assert.IsNull(TeamFortressService.GetMapStatsAsync("bonewards").Result);
        }

        [Test]
        public void GetNewsOverview()
        {
            Assert.IsNotNull(TeamFortressService.GetNewsArticlesAsync().Result);
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
            Assert.IsNotNull(TeamFortressService.GetServersByGameModeAsync("payload").Result);
            Assert.IsNull(TeamFortressService.GetServersByGameModeAsync("payloader").Result);
        }

        [Test]
        [Order(1)]
        public void UpdateTf2Schema()
        {
            Assert.IsTrue(TeamFortressService.UpdateTf2SchemaAsync().Result);
        }
    }
}