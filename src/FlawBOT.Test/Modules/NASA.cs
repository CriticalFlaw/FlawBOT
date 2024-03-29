﻿using FlawBOT.Services;
using NUnit.Framework;

namespace FlawBOT.Test.Modules;

[TestFixture]
internal class NASA
{
    [Test]
    public void GetNasaData()
    {
        Assert.IsNotNull(NasaService.GetNasaImageAsync(TestSetup.Tokens.NasaToken).Result);
    }
}