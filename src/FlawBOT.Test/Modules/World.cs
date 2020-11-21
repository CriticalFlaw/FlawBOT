using System.Net;
using FlawBOT.Services;
using NUnit.Framework;

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