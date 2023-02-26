using FlawBOT.Modules.Twitch;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Twitch
{
    [Test]
    public void GetStreamData()
    {
        Assert.IsNotNull(TwitchService.GetTwitchDataAsync(TestSetup.Tokens.TwitchToken, TestSetup.Tokens.TwitchAccess, "rifftrax"));
    }
}