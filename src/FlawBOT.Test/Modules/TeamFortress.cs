using FlawBOT.Modules.Steam;
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
        Assert.IsNull(TeamFortressService.GetMapStatsAsync(TestSetup.Tokens.TeamworkToken, "bonewards").Result);
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
        Assert.IsNull(TeamFortressService.GetSchemaItem("shattergun"));
    }

    [Test]
    public void GetServers()
    {
        Assert.IsNotNull(TeamFortressService.GetServersByGameModeAsync(TestSetup.Tokens.TeamworkToken, "payload").Result);
        Assert.IsNull(TeamFortressService.GetServersByGameModeAsync(TestSetup.Tokens.TeamworkToken, "payloader").Result);
    }

    [Test]
    [Order(1)]
    public void UpdateTf2Schema()
    {
        Assert.IsTrue(TeamFortressService.UpdateTF2SchemaAsync(TestSetup.Tokens.SteamToken).Result);
    }
}