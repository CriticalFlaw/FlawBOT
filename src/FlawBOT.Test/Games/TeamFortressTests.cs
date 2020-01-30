using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace GamesModule
{
    [TestFixture]
    internal class TeamFortressTests
    {
        [Test, Ignore("API requires key.")]
        public void GetMapStats()
        {
            Assert.IsNotNull(TeamFortressService.GetMapStatsAsync("pl_upward").Result.MapName);
            Assert.IsNotNull(TeamFortressService.GetMapStatsAsync("upward").Result.MapName);
            Assert.IsNull(TeamFortressService.GetMapStatsAsync("onpward").Result.MapName);
        }

        [Test, Ignore("API requires key.")]
        public void GetNewsOverview()
        {
            Assert.IsNotNull(TeamFortressService.GetNewsOverviewAsync().Result);
        }

        [Test, Ignore("API requires key.")]
        public void GetSchemaItem()
        {
            Assert.IsNotNull(TeamFortressService.GetSchemaItem("scattergun"));
            Assert.IsNull(TeamFortressService.GetSchemaItem("shattergun"));
        }

        [Test, Ignore("API requires key.")]
        public void GetServers()
        {
            Assert.Greater(TeamFortressService.GetServersAsync("payload").Result.Count, 0);
            Assert.Equals(TeamFortressService.GetServersAsync("payloader").Result.Count, 0);
        }

        [Test, Ignore("API requires key.")]
        public void UpdateTF2Schema()
        {
            Assert.IsTrue(TeamFortressService.UpdateTF2SchemaAsync().Result);
        }
    }
}