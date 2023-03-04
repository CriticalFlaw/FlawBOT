using FlawBOT.Services;
using NUnit.Framework;
using System.Net;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Misc
{
    [Test]
    public void GetCatFact()
    {
        Assert.IsNotNull(MiscService.GetCatPhotoAsync().Result);
    }

    [Test]
    public void GetDogPhoto()
    {
        Assert.IsNotNull(MiscService.GetDogPhotoAsync());
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