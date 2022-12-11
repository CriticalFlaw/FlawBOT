using FlawBOT.Modules.Wikipedia;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Wikipedia
{
    [Test]
    public void GetWikipediaPage()
    {
        Assert.Greater(WikipediaService.GetWikipediaDataAsync("Russia").Result.QueryResult.SearchResults.Count, 0);
    }
}