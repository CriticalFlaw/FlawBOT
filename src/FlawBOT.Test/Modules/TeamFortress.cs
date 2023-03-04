using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class TeamFortress
{
    [Test]
    public void GetMapStats()
    {
        Assert.IsNotNull(TeamFortressService.GetMapStatsAsync(TestSetup.Tokens.TeamworkToken, "pl_upward").Result);
        Assert.IsNotNull(TeamFortressService.GetMapStatsAsync(TestSetup.Tokens.TeamworkToken, "upward").Result);
    }

    [Test]
    [Timeout(10000)]
    public void GetNewsOverview()
    {
        Assert.IsNotNull(TeamFortressService.GetNewsArticlesAsync(TestSetup.Tokens.TeamworkToken).Result);
    }

    [Test]
    [Order(2)]
    public void GetSchemaItem()
    {
        Assert.IsNotNull(TeamFortressService.GetSchemaItem("scattergun"));
    }

    [Test]
    public void GetServers()
    {
        Assert.IsNotNull(TeamFortressService.GetServersByGameModeAsync(TestSetup.Tokens.TeamworkToken, "payload"));
    }

    [Test]
    [Order(1)]
    public void UpdateTf2Schema()
    {
        Assert.IsTrue(TeamFortressService.UpdateTF2SchemaAsync(TestSetup.Tokens.SteamToken).Result);
    }
}