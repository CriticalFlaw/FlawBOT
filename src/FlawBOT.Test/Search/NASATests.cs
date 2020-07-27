using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class NasaTests
    {
        [Test]
        public void GetNasaData()
        {
            Assert.IsNotNull(NasaService.GetNasaImageAsync().Result);
        }
    }
}