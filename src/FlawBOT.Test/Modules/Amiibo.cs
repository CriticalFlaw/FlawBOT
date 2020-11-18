using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace Modules
{
    [TestFixture]
    internal class Amiibo
    {
        [Test]
        public void GetAmiiboData()
        {
            Assert.IsNotNull(AmiiboService.GetAmiiboDataAsync("Donkey Kong").Result);
            Assert.IsNull(AmiiboService.GetAmiiboDataAsync("Konkey Dong").Result);
        }
    }
}