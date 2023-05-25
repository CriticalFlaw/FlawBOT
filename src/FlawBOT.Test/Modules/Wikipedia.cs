using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Wikipedia
{
    [Test]
    public void GetWikipediaPage()
    {
        Assert.IsNotNull(WikipediaService.GetWikipediaArticlesAsync("Russia"));
    }
}