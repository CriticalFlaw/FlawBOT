using FlawBOT.Framework.Services;
using NUnit.Framework;
using System.Net;

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
        public void GetIPLocation()
        {
            Assert.IsTrue(GoogleService.GetIPLocationAsync(IPAddress.Parse("123.123.123.123")).Result.Status == "success");
        }

        [Test]
        public void GetRandomAnswer()
        {
            Assert.IsNotNull(EightBallService.GetRandomAnswer());
        }
    }
}