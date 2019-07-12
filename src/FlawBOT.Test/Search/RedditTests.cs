using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    internal class RedditTests
    {
        [Test]
        public void GetSubredditData()
        {
            Assert.NotNull(RedditService.GetSubredditAsync("AskReddit"));
        }
    }
}