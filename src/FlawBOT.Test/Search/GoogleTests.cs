using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class GoogleTests
    {
        [Test]
        public void GetTimeData()
        {
            Assert.IsTrue(GoogleService.GetTimeDataAsync("Ottawa").Result.status == "OK");
            Assert.IsTrue(GoogleService.GetTimeDataAsync("Ottura").Result.status != "OK");
        }

        [Test]
        public void GetWeatherData()
        {
            Assert.IsTrue(GoogleService.GetWeatherDataAsync("Ottawa").Result.COD != 404);
            Assert.IsTrue(GoogleService.GetWeatherDataAsync("Ottura").Result.COD == 404);
        }
    }
}