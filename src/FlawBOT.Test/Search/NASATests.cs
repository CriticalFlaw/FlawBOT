using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class NASATests
    {
        [Test, Ignore("API requires key.")]
        public void GetNASAData()
        {
            Assert.IsNotNull(NASAService.GetNASAImageAsync().Result);
        }
    }
}