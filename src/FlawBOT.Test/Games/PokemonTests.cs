using FlawBOT.Services.Games;
using NUnit.Framework;

namespace GamesModule
{
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