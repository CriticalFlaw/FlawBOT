using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class GoogleTests
    {
        [Test]
        public void GetNewsData()
        {
            Assert.IsTrue(GoogleService.GetNewsDataAsync().Result.Status == "ok");
            Assert.IsTrue(GoogleService.GetNewsDataAsync("Nintendo").Result.Status == "ok");
        }

        [Test]
        public void GetTimeData()
        {
            Assert.IsNotNull(GoogleService.GetTimeDataAsync("Ottawa").Result.Status);
            Assert.IsNull(GoogleService.GetTimeDataAsync("Ottura").Result);
        }

        [Test]
        public void GetWeatherData()
        {
            Assert.IsNotNull(GoogleService.GetWeatherDataAsync("Ottawa").Result);
            Assert.IsNull(GoogleService.GetWeatherDataAsync("Ottura").Result);
        }
    }
}