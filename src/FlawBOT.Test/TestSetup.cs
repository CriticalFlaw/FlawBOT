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
        // Load the API tokens from the configuration file.
        var fileName = "Resources\\config.json";
        if (!File.Exists(fileName)) Assert.Inconclusive("Configuration file is not present.");
        var json = new StreamReader(File.OpenRead(fileName), new UTF8Encoding(false)).ReadToEnd();
        Tokens = JsonConvert.DeserializeObject<BotSettings>(json).Tokens;
    }

    [OneTimeTearDown]
    public void PostTest()
    {
        // Nothing...
    }
}