using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class Dictionary
{
    [Test]
    public void GetDictionaryDefinition()
    {
        Assert.IsNotNull(DictionaryService.GetDictionaryDefinitionAsync("computer").Result);
        Assert.IsNull(DictionaryService.GetDictionaryDefinitionAsync("kompuuter").Result);
    }
}