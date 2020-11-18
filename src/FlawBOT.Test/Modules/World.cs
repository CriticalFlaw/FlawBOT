using FlawBOT.Framework.Services;
using NUnit.Framework;
using System.Net;

namespace Modules
{
    [TestFixture]
    internal class World
    {
        [Test]
        public void GetIpLocation()
        {
            Assert.IsNotNull(WorldService.GetIpLocationAsync(IPAddress.Parse("123.123.123.123")).Result.Type);
        }

        [Test]
        public void GetWeatherData()
        {
            Assert.IsNotNull(WorldService.GetWeatherDataAsync("Ottawa").Result);
            Assert.IsNull(WorldService.GetWeatherDataAsync("Ottura").Result);
        }
    }
}