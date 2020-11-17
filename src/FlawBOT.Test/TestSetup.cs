﻿using System.IO;
using System.Text;
using FlawBOT.Framework.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GamesModule
{
    [SetUpFixture]
    internal class TestSetup
    {
        [OneTimeSetUp]
        public void PreTest()
        {
            var json = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)).ReadToEnd();
            TokenHandler.Tokens = JsonConvert.DeserializeObject<TokenData>(json);
        }
    }
}