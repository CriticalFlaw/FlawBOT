using FlawBOT.Modules;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Dictionary
{
    [Test]
    public void GetDictionaryDefinition()
    {
        Assert.Greater(DictionaryService.GetDictionaryDefinitionAsync("computer").Result.Count, 0);
        Assert.IsNull(DictionaryService.GetDictionaryDefinitionAsync("kompuuter").Result);
    }
}