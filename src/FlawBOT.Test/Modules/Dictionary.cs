using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test
{
    [TestFixture]
    internal class Dictionary
    {
        [Test]
        public void GetDictionaryDefinition()
        {
            Assert.Greater(DictionaryService.GetDictionaryDefinitionAsync("computer").Result.List.Count, 0);
            Assert.AreEqual(DictionaryService.GetDictionaryDefinitionAsync("kompuuter").Result.List.Count, 0);
        }
    }
}