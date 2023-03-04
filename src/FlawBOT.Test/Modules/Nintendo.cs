using FlawBOT.Services;
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
        Assert.IsNotNull(NintendoService.GetPokemonCardAsync(TestSetup.Tokens.PokemonToken, "pikachu"));
    }
}