using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class OMDB
{
    [Test]
    public void GetMovieData()
    {
        Assert.IsNotNull(OmdbService.GetOMDBResult(TestSetup.Tokens.OmdbToken, "office+space").Result);
    }
}