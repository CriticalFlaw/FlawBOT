using FlawBOT.Services.Search;
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