using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class RedditTests
    {
        [Test]
        public void GetHotSubredditPost()
        {
            Assert.NotNull(RedditService.GetEmbeddedResults("AskReddit", RedditCategory.Hot));
        }

        [Test]
        public void GetNewSubredditPost()
        {
            Assert.NotNull(RedditService.GetEmbeddedResults("AskReddit", RedditCategory.New));
        }

        [Test]
        public void GetTopSubredditPost()
        {
            Assert.NotNull(RedditService.GetEmbeddedResults("AskReddit", RedditCategory.Top));
        }
    }
}