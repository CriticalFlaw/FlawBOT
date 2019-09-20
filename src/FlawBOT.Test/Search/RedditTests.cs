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
            Assert.NotNull(RedditService.GetSubredditPost("AskReddit", RedditCategory.Hot));
        }

        [Test]
        public void GetNewSubredditPost()
        {
            Assert.NotNull(RedditService.GetSubredditPost("AskReddit", RedditCategory.New));
        }

        [Test]
        public void GetTopSubredditPost()
        {
            Assert.NotNull(RedditService.GetSubredditPost("AskReddit", RedditCategory.Top));
        }
    }
}