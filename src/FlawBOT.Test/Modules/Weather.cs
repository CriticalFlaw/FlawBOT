using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Weather
{
    [Test]
    public void GetWeatherData()
    {
        Assert.IsNotNull(WeatherService.GetWeatherDataAsync(TestSetup.Tokens.WeatherToken, "Toronto"));
    }
}