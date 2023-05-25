using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Simpsons
{
    [Test]
    public void GetFuturamaEpisode()
    {
        Assert.NotNull(SimpsonsService.GetEpisodePostAsync(SimpsonsService.SiteRoot.Morbotron).Result.Title);
    }

    [Test]
    public void GetSimpsonsEpisode()
    {
        Assert.NotNull(SimpsonsService.GetEpisodePostAsync(SimpsonsService.SiteRoot.Frinkiac).Result.Title);
    }
}