using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    [TestFixture]
    internal class DictionaryTests
    {
        [Test]
        public void GetDictionaryDefinition()
        {
            Assert.Greater(DictionaryService.GetDictionaryDefinitionAsync("computer").Result.List.Count, 0);
            Assert.AreEqual(DictionaryService.GetDictionaryDefinitionAsync("kompuuter").Result.List.Count, 0);
        }
    }
}