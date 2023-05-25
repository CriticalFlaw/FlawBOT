using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Nintendo
{
    [Test]
    public void GetAmiiboData()
    {
        Assert.IsNotNull(NintendoService.GetAmiiboInfoAsync("Donkey Kong").Result);
        Assert.IsNull(NintendoService.GetAmiiboInfoAsync("Konkey Dong").Result);
    }

    [Test]
    public void GetPokemonCards()
    {
        Assert.IsNotNull(NintendoService.GetPokemonCardAsync(TestSetup.Tokens.PokemonToken, "pikachu"));
    }
}