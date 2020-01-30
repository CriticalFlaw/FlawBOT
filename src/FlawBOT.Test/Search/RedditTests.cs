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
            Assert.IsNotNull(RedditService.GetEmbeddedResults("AskReddit", RedditCategory.Hot));
            Assert.IsNotNull(RedditService.GetEmbeddedResults("AskReddit", RedditCategory.New));
            Assert.IsNotNull(RedditService.GetEmbeddedResults("AskReddit", RedditCategory.Top));
            Assert.IsTrue(RedditService.GetEmbeddedResults("AskDinosaurs", RedditCategory.Hot).Description.Contains(":warning:"));
            Assert.IsTrue(RedditService.GetEmbeddedResults("AskDinosaurs", RedditCategory.New).Description.Contains(":warning:"));
            Assert.IsTrue(RedditService.GetEmbeddedResults("AskDinosaurs", RedditCategory.Top).Description.Contains(":warning:"));
        }
    }
}