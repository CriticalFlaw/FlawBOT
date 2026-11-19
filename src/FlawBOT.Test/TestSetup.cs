using FlawBOT.Common;

namespace FlawBOT.Test;

[SetUpFixture]
public class TestSetup
{
    /// <summary>
    ///     API tokens for live tests. Read from Resources/config.json, then overridden by FLAWBOT_TOKEN_* environment variables
    ///     (for example FLAWBOT_TOKEN_NASA or FLAWBOT_TOKEN_STEAM) so CI secrets never have to be written to disk.
    /// </summary>
    public static Tokens Tokens { get; private set; } = new();

    [OneTimeSetUp]
    public void PreTest()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "config.json");
        var settings = File.Exists(path) ? Program.LoadSettings(File.ReadAllText(path)) : new BotSettings();
        Tokens = settings.Tokens;

        foreach (var property in typeof(Tokens).GetProperties())
        {
            var name = property.Name.Replace("Token", string.Empty, StringComparison.Ordinal).ToUpperInvariant();
            var value = Environment.GetEnvironmentVariable($"FLAWBOT_TOKEN_{name}");
            if (!string.IsNullOrWhiteSpace(value)) property.SetValue(Tokens, value);
        }

        Program.Settings = settings;
    }

    /// <summary>
    ///     Skips a live test when the API token it needs has not been configured.
    /// </summary>
    public static string RequireToken(string token, string name)
    {
        if (string.IsNullOrWhiteSpace(token))
            Assert.Ignore($"No {name} token configured. Set it in config.json or the FLAWBOT_TOKEN_{name.ToUpperInvariant()} environment variable.");
        return token;
    }
}
