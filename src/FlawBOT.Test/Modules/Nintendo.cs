using FlawBOT.Services.Games;
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
        Assert.Greater(NintendoService.GetPokemonCardsAsync("pikachu").Result.Count, 0);
        Assert.AreEqual(NintendoService.GetPokemonCardsAsync("rikachu").Result.Count, 0);
    }
}