using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace GamesModule
{
    [TestFixture]
    internal class PokemonTests
    {
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