using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace Modules
{
    [TestFixture]
    internal class Misc
    {
        [Test]
        public void GetCatFact()
        {
            Assert.IsNotNull(MiscService.GetCatFactAsync().Result);
        }

        [Test]
        public void GetDogPhoto()
        {
            Assert.IsTrue(MiscService.GetDogPhotoAsync().Result.Status == "success");
        }

        [Test]
        public void GetRandomAnswer()
        {
            Assert.IsNotNull(MiscService.GetRandomAnswer());
        }
    }
}