using System.Net;
using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace MiscModule
{
    [TestFixture]
    internal class MiscTests
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
        public void GetIpLocation()
        {
            Assert.IsNotNull(MiscService.GetIpLocationAsync(IPAddress.Parse("123.123.123.123")).Result.Type);
        }

        [Test]
        public void GetRandomAnswer()
        {
            Assert.IsNotNull(MiscService.GetRandomAnswer());
        }
    }
}