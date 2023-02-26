using FlawBOT.Modules.Misc;
using NUnit.Framework;
using System.Net;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Misc
{
    [Test]
    public void GetCatFact()
    {
        Assert.IsNotNull(MiscService.GetCatFactAsync().Result);
    }

    [Test]
    public void GetDogPhoto()
    {
        Assert.IsTrue(MiscService.GetDogPhotoAsync().Result.Status == "success");
    }

    [Test]
    public void GetRandomAnswer()
    {
        Assert.IsNotNull(MiscService.GetRandomAnswer());
    }

    [Test]
    public void GetIpLocation()
    {
        Assert.IsNotNull(MiscService.GetIpLocationAsync(IPAddress.Parse("123.123.123.123")).Result);
    }
}