using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace Modules
{
    [TestFixture]
    internal class NASA
    {
        [Test]
        public void GetNasaData()
        {
            Assert.IsNotNull(NasaService.GetNasaImageAsync().Result);
        }
    }
}