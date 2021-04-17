using System.IO;
using FlawBOT.Common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GamesModule
{
    [SetUpFixture]
    internal class TestSetup
    {
        public static BotSettings Settings { get; set; }

        [OneTimeSetUp]
        public void PreTest()
        {
            Settings = JsonConvert.DeserializeObject<BotSettings>(File.ReadAllText("config.json"));
        }
    }
}