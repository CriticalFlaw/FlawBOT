using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class GoogleTests
    {
        [Test, Ignore("API requires key.")]
        public void GetTimeData()
        {
            Assert.IsTrue(GoogleService.GetTimeDataAsync("Ottawa").Result.status == "OK");
            Assert.IsFalse(GoogleService.GetTimeDataAsync("Ottura").Result.status == "OK");
        }

        [Test, Ignore("API requires key.")]
        public void GetWeatherData()
        {
            Assert.IsTrue(GoogleService.GetWeatherDataAsync("Ottawa").Result.COD == 404);
            Assert.IsFalse(GoogleService.GetWeatherDataAsync("Ottura").Result.COD == 404);
        }

        [Test, Ignore("API requires key.")]
        public void GetNewsData()
        {
            Assert.IsTrue(GoogleService.GetNewsDataAsync().Result.Status == "ok");
            Assert.IsTrue(GoogleService.GetNewsDataAsync("Nintendo").Result.Status == "ok");
        }
    }
}