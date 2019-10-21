using FlawBOT.Framework.Services;
using NUnit.Framework;
using System.Net;
using System.Net.Http;

namespace MiscModule
{
    [TestFixture]
    internal class MiscTests
    {
        [Test]
        public void GetAnswer()
        {
            Assert.IsNotEmpty(EightBallService.GetAnswer());
        }

        [Test]
        public void GetCatFact()
        {
            var http = new HttpClient();
            Assert.IsNotNull(http.GetStringAsync("https://catfact.ninja/fact"));
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
    }
}