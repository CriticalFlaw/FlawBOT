using FlawBOT.Services;
using NUnit.Framework;
using System.Net.Http;

namespace MiscModule
{
    internal class MiscTests
    {
        [Test]
        public void Get8BallAnswer()
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

        // TODO: Add Coin-Flip, Color, Dice Roll
    }
}