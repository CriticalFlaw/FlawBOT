using FlawBOT.Modules;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Reddit
{
    [Test]
    public void GetEmbeddedResults()
    {
        Assert.IsNotNull(RedditService.GetResults("AskReddit", RedditCategory.Hot));
        Assert.IsNotNull(RedditService.GetResults("AskReddit", RedditCategory.New));
        Assert.IsNotNull(RedditService.GetResults("AskReddit", RedditCategory.Top));
    }
}