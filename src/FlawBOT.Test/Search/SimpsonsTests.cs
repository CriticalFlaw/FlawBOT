using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    internal class SimpsonsTests
    {
        [Test]
        public void GetSimpsonsPhoto()
        {
            Assert.NotNull(SimpsonsService.GetSimpsonsDataAsync("frinkiac").Result.Title);
        }
    }
}