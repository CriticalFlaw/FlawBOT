using FlawBOT.Modules;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Simpsons
{
    [Test]
    public void GetFuturamaEpisode()
    {
        Assert.NotNull(SimpsonsService.GetEpisodeGifAsync(SimpsonsService.SiteRoot.Morbotron).Result.Title);
    }

    [Test]
    public void GetSimpsonsEpisode()
    {
        Assert.NotNull(SimpsonsService.GetEpisodeGifAsync(SimpsonsService.SiteRoot.Frinkiac).Result.Title);
    }
}