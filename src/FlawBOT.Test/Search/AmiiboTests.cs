using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class AmiiboTests
    {
        [Test]
        public void GetAmiiboData()
        {
            Assert.IsNotNull(AmiiboService.GetAmiiboDataAsync("Donkey Kong").Result);
            Assert.IsNull(AmiiboService.GetAmiiboDataAsync("Konkey Dong").Result);
        }
    }
}