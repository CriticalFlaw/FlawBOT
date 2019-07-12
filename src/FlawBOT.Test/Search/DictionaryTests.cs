﻿using FlawBOT.Framework.Services;
using NUnit.Framework;

namespace SearchModule
{
    internal class DictionaryTests
    {
        [Test]
        public void GetDictionaryDefinition()
        {
            Assert.IsFalse(DictionaryService.GetDictionaryForTermAsync("computer").Result.ResultType == "no_results");
        }
    }
}