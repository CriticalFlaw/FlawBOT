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
            Assert.IsNotNull(CatService.GetCatFactAsync().Result);
        }

        [Test]
        public void GetDogPhoto()
        {
            Assert.IsTrue(DogService.GetDogPhotoAsync().Result.Status == "success");
        }

        [Test]
        public void GetIpLocation()
        {
            Assert.IsTrue(GoogleService.GetIpLocationAsync(IPAddress.Parse("123.123.123.123")).Result.Status == "success");
        }

        [Test]
        public void GetRandomAnswer()
        {
            Assert.IsNotNull(EightBallService.GetRandomAnswer());
        }
    }
}