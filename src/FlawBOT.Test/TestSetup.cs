using System.IO;
using System.Text;
using FlawBOT.Common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FlawBOT.Test
{
    [SetUpFixture]
    public class TestSetup
    {
        public static Tokens Tokens { get; set; }

        [OneTimeSetUp]
        public void PreTest()
        {
            if (!File.Exists("config.json")) Assert.Inconclusive("Configuration file is not present.");
            var json = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)).ReadToEnd();
            Tokens = JsonConvert.DeserializeObject<BotSettings>(json)?.Tokens;
        }

        [OneTimeTearDown]
        public void PostTest()
        {
            // Nothing...
        }
    }
}