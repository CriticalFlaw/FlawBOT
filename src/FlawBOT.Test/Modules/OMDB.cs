﻿using FlawBOT.Services.Lookup;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class OMDB
{
    [Test]
    public void GetMovieData()
    {
        Assert.IsNotNull(OmdbService.GetMovieDataAsync(TestSetup.Tokens.OmdbToken, "office+space").Result);
    }
}