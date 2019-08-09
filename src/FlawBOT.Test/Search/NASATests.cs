using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace FlawBOT.Test.Search
{
    [TestFixture]
    internal class NASATests
    {
        [Test]
        public void GetNASAData()
        {
            Assert.IsNotNull(NASAService.GetNASAImage().Result);
        }
    }
}