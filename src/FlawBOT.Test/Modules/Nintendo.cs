using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test
{
    [TestFixture]
    internal class Nintendo
    {
        [Test]
        public void GetAmiiboData()
        {
            Assert.IsNotNull(AmiiboService.GetAmiiboDataAsync("Donkey Kong").Result);
            Assert.IsNull(AmiiboService.GetAmiiboDataAsync("Konkey Dong").Result);
        }

        [Test]
        public void GetPokemonCards()
        {
            Assert.Greater(PokemonService.GetPokemonCardsAsync().Result.Cards.Count, 0);
            Assert.Greater(PokemonService.GetPokemonCardsAsync("pikachu").Result.Cards.Count, 0);
            Assert.AreEqual(PokemonService.GetPokemonCardsAsync("rikachu").Result.Cards.Count, 0);
        }

        [Test]
        public void UpdatePokemonList()
        {
            Assert.IsTrue(PokemonService.UpdatePokemonListAsync().Result);
        }
    }
}