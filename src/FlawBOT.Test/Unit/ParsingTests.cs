using DSharpPlus;
using FlawBOT.Modules;
using FlawBOT.Services;

namespace FlawBOT.Test.Unit;

[TestFixture]
[Category("Unit")]
internal class ParsingTests
{
    [TestCase("192.168.22.11:27015", "steam://connect/192.168.22.11:27015")]
    [TestCase("  192.168.22.11:27015  ", "steam://connect/192.168.22.11:27015")]
    [TestCase("192.168.22.11", "steam://connect/192.168.22.11")]
    [TestCase("174.116.36.120:27015; password hello", "steam://connect/174.116.36.120:27015/hello")]
    [TestCase("connect tf2.example.com:27015; password \"hello\"", "steam://connect/tf2.example.com:27015/hello")]
    public void FormatConnectLink(string input, string expected)
    {
        Assert.That(SteamService.FormatConnectLink(input), Is.EqualTo(expected));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("not an address at all")]
    [TestCase("1.2.3.4:27015; rm -rf")]
    public void FormatConnectLinkRejectsInvalidInput(string input)
    {
        Assert.That(SteamService.FormatConnectLink(input), Is.Null);
    }

    [TestCase("76561197960287930", null, 76561197960287930UL)]
    [TestCase("https://steamcommunity.com/profiles/76561197960287930/", null, 76561197960287930UL)]
    [TestCase("https://steamcommunity.com/id/criticalflaw", "criticalflaw", null)]
    [TestCase("critical flaw", "criticalflaw", null)]
    [TestCase("12345", "12345", null)]
    public void ParseSteamUserQuery(string input, string expectedVanity, ulong? expectedId)
    {
        var vanity = SteamService.ParseSteamUserQuery(input, out var steamId);
        Assert.Multiple(() =>
        {
            Assert.That(vanity, Is.EqualTo(expectedVanity));
            Assert.That(steamId, Is.EqualTo(expectedId));
        });
    }

    [Test]
    public async Task SteamIdsResolveWithoutAnApiCall()
    {
        Assert.That(await SteamService.GetSteamUserIdAsync(null, "76561197960287930"), Is.EqualTo(76561197960287930UL));
    }

    [TestCase("aww", "aww")]
    [TestCase("r/AskReddit", "askreddit")]
    [TestCase("/r/dotnet/", "dotnet")]
    [TestCase("/r/dotnet", "dotnet")]
    [TestCase("not a subreddit", null)]
    [TestCase("a", null)]
    [TestCase("../../etc", null)]
    [TestCase(null, null)]
    public void NormalizeSubreddit(string input, string expected)
    {
        Assert.That(RedditService.NormalizeSubreddit(input), Is.EqualTo(expected));
    }

    [TestCase(RedditCategory.Hot, "hot")]
    [TestCase(RedditCategory.New, "new")]
    [TestCase(RedditCategory.Top, "top")]
    public void RedditCategoryPath(RedditCategory category, string expected)
    {
        Assert.That(RedditService.GetCategoryPath(category), Is.EqualTo(expected));
    }

    [TestCase("My Channel", ChannelType.Text, "my-channel")]
    [TestCase("  general  ", ChannelType.Text, "general")]
    [TestCase("Music Room", ChannelType.Voice, "Music Room")]
    public void NormalizeChannelName(string input, ChannelType type, string expected)
    {
        Assert.That(ChannelModule.NormalizeChannelName(input, type), Is.EqualTo(expected));
    }

    [TestCase(0, 32)]
    [TestCase(100, 212)]
    [TestCase(-40, -40)]
    [TestCase(21.5, 70.7)]
    public void CelsiusToFahrenheit(double celsius, double fahrenheit)
    {
        Assert.That(WeatherService.CelsiusToFahrenheit(celsius), Is.EqualTo(fahrenheit).Within(0.0001));
    }

    [Test]
    public void ToDurationString()
    {
        Assert.Multiple(() =>
        {
            Assert.That(MusicService.ToDurationString(TimeSpan.FromSeconds(5)), Is.EqualTo("0:05"));
            Assert.That(MusicService.ToDurationString(new TimeSpan(0, 3, 30)), Is.EqualTo("3:30"));
            Assert.That(MusicService.ToDurationString(new TimeSpan(1, 2, 3)), Is.EqualTo("1:02:03"));
        });
    }

    [Test]
    public void MagicBallAnswersAreFromTheList()
    {
        for (var i = 0; i < 50; i++)
            Assert.That(MiscService.Answers, Does.Contain(MiscService.GetRandomAnswer()));
    }
}
