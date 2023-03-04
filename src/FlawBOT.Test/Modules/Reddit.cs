using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Reddit
{
    [Test]
    public void GetEmbeddedResults()
    {
        Assert.IsNotNull(RedditService.GetRedditResults("AskReddit", RedditCategory.Hot));
        Assert.IsNotNull(RedditService.GetRedditResults("AskReddit", RedditCategory.New));
        Assert.IsNotNull(RedditService.GetRedditResults("AskReddit", RedditCategory.Top));
    }
}