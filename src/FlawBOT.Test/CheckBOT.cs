using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test
{
    internal class CheckBOT
    {
        [Test]
        public void EightBall()
        {
            Assert.IsNotEmpty(EightBallService.GetAnswer());
        }
    }
}