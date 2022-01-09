using FlawBOT.Services.Lookup;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Wikipedia
{
    [Test]
    public void GetWikipediaPage()
    {
        Assert.IsNull(WikipediaService.GetWikipediaDataAsync("Russia").Error);
    }
}