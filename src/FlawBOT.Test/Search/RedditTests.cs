using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class RedditTests
    {
        [Test]
        public void GetEmbeddedResults()
        {
            Assert.IsNotNull(RedditService.GetResults("AskReddit", RedditCategory.Hot));
            Assert.IsNotNull(RedditService.GetResults("AskReddit", RedditCategory.New));
            Assert.IsNotNull(RedditService.GetResults("AskReddit", RedditCategory.Top));
        }
    }
}