using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Services;

namespace FlawBOT.Test.Unit;

[TestFixture]
[Category("Unit")]
internal class EmbedExtensionsTests
{
    [TestCase(null, 10, null)]
    [TestCase("", 10, "")]
    [TestCase("short", 10, "short")]
    [TestCase("exactly10!", 10, "exactly10!")]
    [TestCase("this is far too long", 10, "this is...")]
    public void Truncate(string input, int length, string expected)
    {
        Assert.That(input.Truncate(length), Is.EqualTo(expected));
    }

    [Test]
    public void TruncateNeverExceedsLimit()
    {
        var value = new string('a', 5000);
        Assert.That(value.Truncate(EmbedExtensions.FieldValueLimit), Has.Length.EqualTo(EmbedExtensions.FieldValueLimit));
    }

    [Test]
    public void StripHtmlRemovesTagsAndDecodesEntities()
    {
        Assert.That("<p>Fish &amp; <b>Chips</b></p> &quot;yum&quot;".StripHtml(), Is.EqualTo("Fish & Chips \"yum\""));
    }

    [Test]
    public void JoinOrDefaultSkipsBlanks()
    {
        Assert.Multiple(() =>
        {
            Assert.That(new[] { "a", "", null, "b" }.JoinOrDefault(), Is.EqualTo("a, b"));
            Assert.That(new[] { "", " " }.JoinOrDefault(), Is.EqualTo(EmbedExtensions.Unknown));
            Assert.That(((string[])null).JoinOrDefault(", ", "None"), Is.EqualTo("None"));
        });
    }

    [Test]
    public void JoinWithinLimitDropsWholeItems()
    {
        var items = Enumerable.Range(0, 100).Select(i => $"item{i:000}");
        var result = items.JoinWithinLimit(", ", 30);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.LessThanOrEqualTo(30));
            Assert.That(result, Is.EqualTo("item000, item001, item002"));
        });
    }

    [Test]
    public void AddSafeFieldHandlesBlankAndOversizedValues()
    {
        var embed = new DiscordEmbedBuilder()
            .AddSafeField("Blank", null)
            .AddSafeField("", "value")
            .AddSafeField("Long", new string('x', 3000))
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(embed.Fields[0].Value, Is.EqualTo(EmbedExtensions.Unknown));
            Assert.That(embed.Fields[1].Name, Is.Not.Empty);
            Assert.That(embed.Fields[2].Value, Has.Length.EqualTo(EmbedExtensions.FieldValueLimit));
        });
    }

    [Test]
    public void AddSafeFieldStopsAtDiscordFieldLimit()
    {
        var builder = new DiscordEmbedBuilder();
        for (var i = 0; i < 30; i++) builder.AddSafeField($"Field {i}", "value");
        Assert.That(builder.Fields, Has.Count.EqualTo(25));
    }
}

[TestFixture]
[Category("Unit")]
internal class BotServicesTests
{
    [TestCase(ResponseType.Default, "hello")]
    [TestCase(ResponseType.Warning, ":exclamation: hello")]
    [TestCase(ResponseType.Missing, ":mag: hello")]
    [TestCase(ResponseType.Error, ":no_entry: hello")]
    public void FormatResponse(ResponseType type, string expected)
    {
        Assert.That(BotServices.FormatResponse("hello", type), Is.EqualTo(expected));
    }

    [Test]
    public void FormatResponseFitsInADiscordMessage()
    {
        Assert.That(BotServices.FormatResponse(new string('x', 5000), ResponseType.Error), Has.Length.EqualTo(EmbedExtensions.MessageLimit));
    }

    [TestCase(-5, 1)]
    [TestCase(0, 1)]
    [TestCase(1, 1)]
    [TestCase(50, 50)]
    [TestCase(50.7, 50)]
    [TestCase(100, 100)]
    [TestCase(1000, 100)]
    [TestCase(double.NaN, 1)]
    public void LimitToRange(double input, int expected)
    {
        Assert.That(BotServices.LimitToRange(input), Is.EqualTo(expected));
    }

    [TestCase("https://example.com/image.png", true)]
    [TestCase("http://example.com/image.jpg", true)]
    [TestCase("  https://example.com/a.gif  ", true)]
    [TestCase("ftp://example.com/image.png", false)]
    [TestCase("file:///etc/passwd", false)]
    [TestCase("not a url", false)]
    [TestCase("", false)]
    [TestCase(null, false)]
    public void IsWebUrl(string input, bool expected)
    {
        Assert.That(BotServices.IsWebUrl(input, out _), Is.EqualTo(expected));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("not a url")]
    [TestCase("file:///etc/passwd")]
    public async Task CheckImageInputRejectsInvalidUrls(string input)
    {
        Assert.That(await BotServices.CheckImageInput(input), Is.Null);
    }

    [TestCase("general", true)]
    [TestCase("  ", false)]
    [TestCase(null, false)]
    public void CheckChannelName(string input, bool expected)
    {
        Assert.That(BotServices.CheckChannelName(input), Is.EqualTo(expected));
    }

    [Test]
    public void CheckChannelNameRejectsLongNames()
    {
        Assert.Multiple(() =>
        {
            Assert.That(BotServices.CheckChannelName(new string('a', 100)), Is.True);
            Assert.That(BotServices.CheckChannelName(new string('a', 101)), Is.False);
        });
    }

    [Test]
    public void FormatUptime()
    {
        Assert.Multiple(() =>
        {
            Assert.That(BotServices.FormatUptime(new TimeSpan(0, 1, 2, 3)), Is.EqualTo("01:02:03"));
            Assert.That(BotServices.FormatUptime(new TimeSpan(1, 0, 0, 5)), Is.EqualTo("00:00:05 (1 day)"));
            Assert.That(BotServices.FormatUptime(new TimeSpan(3, 23, 59, 59)), Is.EqualTo("23:59:59 (3 days)"));
        });
    }

    [Test]
    public void FormatUserNameHandlesNullUsers()
    {
        Assert.That(BotServices.FormatUserName(null), Is.EqualTo(EmbedExtensions.Unknown));
    }
}

[TestFixture]
[Category("Unit")]
internal class SettingsTests
{
    [Test]
    public void ShippedConfigurationParses()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "config.json");
        Assert.That(File.Exists(path), Is.True, "config.json should be copied next to the binaries.");

        var settings = Program.LoadSettings(File.ReadAllText(path));
        Assert.Multiple(() =>
        {
            Assert.That(settings.Name, Is.EqualTo("FlawBOT"));
            Assert.That(settings.ShardCount, Is.EqualTo(1));
            Assert.That(settings.Lavalink.Enabled, Is.False);
            Assert.That(settings.Lavalink.Port, Is.EqualTo(2333));
            Assert.That(settings.Tokens, Is.Not.Null);
        });
    }

    [Test]
    public void MissingSectionsFallBackToDefaults()
    {
        var settings = Program.LoadSettings("""{ "ShardCount": 0, "Name": "", "Tokens": null }""");
        Assert.Multiple(() =>
        {
            Assert.That(settings.Name, Is.EqualTo("FlawBOT"));
            Assert.That(settings.ShardCount, Is.EqualTo(1));
            Assert.That(settings.Tokens, Is.Not.Null);
            Assert.That(settings.Lavalink, Is.Not.Null);
            Assert.That(settings.Lavalink.Address, Is.EqualTo("127.0.0.1"));
        });
    }

    [Test]
    public void TokensAreReadByTheirConfigNames()
    {
        var settings = Program.LoadSettings("""{ "Tokens": { "Discord": "d", "OMDB": "o", "TeamworkTF": "t", "TwitchAccess": "ta" } }""");
        Assert.Multiple(() =>
        {
            Assert.That(settings.Tokens.DiscordToken, Is.EqualTo("d"));
            Assert.That(settings.Tokens.OmdbToken, Is.EqualTo("o"));
            Assert.That(settings.Tokens.TeamworkToken, Is.EqualTo("t"));
            Assert.That(settings.Tokens.TwitchAccess, Is.EqualTo("ta"));
        });
    }

    [Test]
    public void VersionIsReported()
    {
        Assert.That(new BotSettings().Version, Is.EqualTo("5.0.0"));
    }
}

[TestFixture]
[Category("Unit")]
internal class ExceptionsTests
{
    [Test]
    public void UnknownErrorsDoNotLeakDetails()
    {
        var (message, type) = Exceptions.Describe(new NullReferenceException("Object reference not set"), "test");
        Assert.Multiple(() =>
        {
            Assert.That(type, Is.EqualTo(ResponseType.Error));
            Assert.That(message, Does.Not.Contain("Object reference"));
        });
    }

    [Test]
    public void ArgumentErrorsPointToTheCommand()
    {
        var (message, type) = Exceptions.Describe(new ArgumentException("bad"), "user ban");
        Assert.Multiple(() =>
        {
            Assert.That(type, Is.EqualTo(ResponseType.Warning));
            Assert.That(message, Does.Contain("/user ban"));
        });
    }

    [Test]
    public void InvalidOperationMessagesAreShown()
    {
        var (message, type) = Exceptions.Describe(new InvalidOperationException("Not allowed here."), "test");
        Assert.Multiple(() =>
        {
            Assert.That(type, Is.EqualTo(ResponseType.Warning));
            Assert.That(message, Is.EqualTo("Not allowed here."));
        });
    }

    [Test]
    public void FailedChecksAreExplained()
    {
        var exception = new SlashExecutionChecksFailedException
        {
            FailedChecks = new List<SlashCheckBaseAttribute> { new DSharpPlus.SlashCommands.Attributes.SlashRequireOwnerAttribute() }.AsReadOnly()
        };

        var (message, type) = Exceptions.Describe(exception, "bot avatar");
        Assert.Multiple(() =>
        {
            Assert.That(type, Is.EqualTo(ResponseType.Error));
            Assert.That(message, Does.Contain("bot avatar"));
            Assert.That(message, Does.Contain("reserved only for the bot owner"));
        });
    }

    [Test]
    public void DiscordPermissionErrorsAreFriendly()
    {
        var exception = (UnauthorizedException)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(UnauthorizedException));
        var (_, type) = Exceptions.Describe(exception, "test");
        Assert.That(type, Is.EqualTo(ResponseType.Warning));
    }
}
