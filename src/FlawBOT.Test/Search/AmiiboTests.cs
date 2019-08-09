using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace GamesModule
{
    [TestFixture]
    internal class AmiiboTests
    {
        [Test]
        public void AmiiboTest()
        {
            Assert.IsNotNull(AmiiboService.GetAmiiboFigurineAsync("Luigi").Result);
        }
    }
}