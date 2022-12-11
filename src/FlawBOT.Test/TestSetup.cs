using FlawBOT.Common;
using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using System.Text;

namespace FlawBOT.Test;

[SetUpFixture]
public class TestSetup
{
    public static Tokens Tokens { get; set; }

    [OneTimeSetUp]
    public void PreTest()
    {
        // Check that the configuration file is present.
        if (!File.Exists("config.json")) Assert.Inconclusive("Configuration file is not present.");

        // Load the API tokens from the configuration file.
        Tokens = JsonConvert.DeserializeObject<BotSettings>(new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)).ReadToEnd())?.Tokens;
    }

    [OneTimeTearDown]
    public void PostTest()
    {
        // Nothing...
    }
}