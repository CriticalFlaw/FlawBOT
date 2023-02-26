using FlawBOT.Modules.Nintendo;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Nintendo
{
    [Test]
    public void GetAmiiboData()
    {
        Assert.IsNotNull(NintendoService.GetAmiiboDataAsync("Donkey Kong").Result);
        Assert.IsNull(NintendoService.GetAmiiboDataAsync("Konkey Dong").Result);
    }

    [Test]
    public void GetPokemonCards()
    {
        Assert.Greater(NintendoService.GetPokemonCardsAsync(TestSetup.Tokens.PokemonToken, "pikachu").Result.Count, 0);
        Assert.AreEqual(NintendoService.GetPokemonCardsAsync(TestSetup.Tokens.PokemonToken, "rikachu").Result.Count, 0);
    }
}