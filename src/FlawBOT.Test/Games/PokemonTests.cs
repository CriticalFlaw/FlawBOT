using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace GamesModule
{
    [TestFixture]
    internal class PokemonTests
    {
        [Test]
        public void LoadPokemonList()
        {
            Assert.IsTrue(PokemonService.UpdatePokemonListAsync().Result);
        }

        [Test]
        public void GetRandomPokemon()
        {
            Assert.IsNotNull(PokemonService.GetPokemonCardsAsync("").Result);
        }
    }
}