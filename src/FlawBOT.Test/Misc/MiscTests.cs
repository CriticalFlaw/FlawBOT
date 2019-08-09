using System.Net.Http;
using FlawBOT.Framework.Services;
using NUnit.Framework;

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

        // TODO: Put tests here...
    }
}