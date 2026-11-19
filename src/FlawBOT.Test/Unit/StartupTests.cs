using FlawBOT.Common;
using FlawBOT.Services;
using Lavalink4NET;
using Microsoft.Extensions.DependencyInjection;

namespace FlawBOT.Test.Unit;

/// <summary>
///     Builds the bot without connecting to Discord to make sure the dependency injection graph is complete.
/// </summary>
[TestFixture]
[Category("Unit")]
[NonParallelizable]
internal class StartupTests
{
    private BotSettings _original;

    [SetUp]
    public void SetUp()
    {
        _original = Program.Settings;
    }

    [TearDown]
    public void TearDown()
    {
        Program.Settings = _original;
    }

    private static BotSettings CreateSettings(bool lavalink)
    {
        var settings = Program.LoadSettings("{}");
        // A syntactically valid but fake token; the client is never connected.
        settings.Tokens.DiscordToken = "MTAwMDAwMDAwMDAwMDAwMDAw.GAAAAA.AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
        settings.Lavalink.Enabled = lavalink;
        return settings;
    }

    [Test]
    public async Task BotBuildsWithoutLavalink()
    {
        Program.Settings = CreateSettings(false);
        var bot = new FlawBot();
        try
        {
            Assert.Multiple(() =>
            {
                Assert.That(bot.Services.GetService<YoutubeService>(), Is.Not.Null);
                Assert.That(bot.Services.GetService<MusicService>(), Is.Null, "Music is only available when Lavalink is enabled.");
            });
        }
        finally
        {
            await bot.Services.DisposeAsync();
        }
    }

    [Test]
    public async Task BotBuildsWithLavalink()
    {
        Program.Settings = CreateSettings(true);
        var bot = new FlawBot();
        try
        {
            Assert.Multiple(() =>
            {
                Assert.That(bot.Services.GetService<IAudioService>(), Is.Not.Null);
                Assert.That(bot.Services.GetService<MusicService>(), Is.Not.Null);
            });
        }
        finally
        {
            await bot.Services.DisposeAsync();
        }
    }
}
