using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace GamesModule
{
    [TestFixture]
    internal class TeamFortressTests
    {
        [Test]
        public void LoadItemSchema()
        {
            Assert.IsTrue(TeamFortressService.LoadTF2SchemaAsync().Result);
        }

        [Test]
        public void GetTeamworkMap()
        {
            Assert.IsNotNull(TeamFortressService.GetMapStatsAsync("pl_upward").Result.MapName);
        }

        [Test]
        public void GetTeamworkNews()
        {
            Assert.IsNotNull(TeamFortressService.GetNewsOverviewAsync().Result);
        }

        [Test]
        public void GetTeamworkServer()
        {
            Assert.IsNotNull(TeamFortressService.GetServersAsync("payload").Result);
        }
    }
}