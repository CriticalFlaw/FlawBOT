using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Speedrun
{
    [Test]
    public void GetSpeedrunGame()
    {
        Assert.IsNotNull(SpeedrunService.GetSpeedrunGameAsync("Wind Waker"));
    }
}