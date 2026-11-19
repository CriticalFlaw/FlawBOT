using FlawBOT.Services;
using static FlawBOT.Test.TestSetup;

namespace FlawBOT.Test.Live;

/// <summary>
///     Calls the real third-party APIs. Excluded from the default test run (see test.runsettings); run them with:
///     dotnet test src/FlawBOT.sln -- NUnit.Where="cat == Live"
///     Tests that need an API token are skipped when the token isn't configured.
/// </summary>
[TestFixture]
[Category("Live")]
[CancelAfter(60_000)]
internal class LiveApiTests
{
    [Test]
    public async Task Dictionary()
    {
        Assert.That(await DictionaryService.GetDictionaryDefinitionAsync("computer"), Is.Not.Null);
        Assert.That(await DictionaryService.GetDictionaryDefinitionAsync(""), Is.Not.Null, "Random definition");
        Assert.That(await DictionaryService.GetDictionaryDefinitionAsync("kompuuterzzqx"), Is.Null);
    }

    [Test]
    public async Task Imgur()
    {
        var token = RequireToken(Tokens.ImgurToken, "Imgur");
        Assert.That(await ImgurService.GetImgurResultAsync(token, "cats"), Is.Not.Null);
    }

    [Test]
    public async Task CatImage()
    {
        Assert.That(await MiscService.GetCatImageAsync(), Is.Not.Null);
    }

    [Test]
    public async Task DogImage()
    {
        Assert.That(await MiscService.GetDogImageAsync(), Is.Not.Null);
    }

    [Test]
    public async Task Nasa()
    {
        // Falls back to NASA's rate-limited DEMO_KEY when no token is configured.
        Assert.That(await NasaService.GetNasaImageAsync(Tokens.NasaToken), Is.Not.Null);
    }

    [Test]
    public async Task News()
    {
        var token = RequireToken(Tokens.NewsToken, "News");
        Assert.That(await NewsService.GetNewsArticlesAsync(token, "bitcoin"), Is.Not.Null);
    }

    [Test]
    public async Task Amiibo()
    {
        Assert.That(await NintendoService.GetAmiiboInfoAsync("Donkey Kong"), Is.Not.Null);
        Assert.That(await NintendoService.GetAmiiboInfoAsync("Konkey Dong"), Is.Null);
    }

    [Test]
    public async Task PokemonCard()
    {
        // The Pokémon TCG API works without a key, with a lower rate limit.
        Assert.That(await NintendoService.GetPokemonCardAsync(Tokens.PokemonToken, "pikachu"), Is.Not.Null);
    }

    [Test]
    public async Task Omdb()
    {
        var token = RequireToken(Tokens.OmdbToken, "OMDB");
        Assert.That(await OmdbService.GetOMDBDataAsync(token, "Office Space"), Is.Not.Null);
        Assert.That(await OmdbService.GetOMDBDataAsync(token, "zzqx not a real movie zzqx"), Is.Null);
    }

    [TestCase(RedditCategory.Hot)]
    [TestCase(RedditCategory.New)]
    [TestCase(RedditCategory.Top)]
    public async Task Reddit(RedditCategory category)
    {
        Assert.That(await RedditService.GetRedditResultsAsync("AskReddit", category), Is.Not.Null);
    }

    [TestCase(SimpsonsService.SiteRoot.Frinkiac)]
    [TestCase(SimpsonsService.SiteRoot.Morbotron)]
    public async Task Simpsons(SimpsonsService.SiteRoot site)
    {
        Assert.That((await SimpsonsService.GetEpisodePostAsync(site))?.Title, Is.Not.Null);
    }

    [Test]
    public async Task Speedrun()
    {
        Assert.That(await SpeedrunService.GetSpeedrunGameAsync("Wind Waker"), Is.Not.Null);
    }

    [Test]
    public async Task SteamGame()
    {
        // Uses the store search when the app list hasn't been downloaded.
        Assert.That(await SteamService.GetSteamGameAsync(Tokens.SteamToken, "Team Fortress 2"), Is.Not.Null);
    }

    [Test]
    public async Task SteamAppList()
    {
        var token = RequireToken(Tokens.SteamToken, "Steam");
        Assert.That(await SteamService.UpdateSteamAppListAsync(token), Is.True);
    }

    [Test]
    public async Task SteamProfile()
    {
        var token = RequireToken(Tokens.SteamToken, "Steam");
        Assert.That(await SteamService.GetSteamProfileAsync(token, "criticalflaw"), Is.Not.Null);
        Assert.That(await SteamService.GetSteamProfileAsync(token, "76561197960287930"), Is.Not.Null);
        Assert.That(await SteamService.GetSteamProfileAsync(token, "99999999999999999"), Is.Null);
    }

    [Test]
    public async Task Tf2Schema()
    {
        var token = RequireToken(Tokens.SteamToken, "Steam");
        Assert.That(await TeamFortressService.UpdateTF2SchemaAsync(token), Is.True);
        Assert.That(TeamFortressService.GetSchemaItem("scattergun"), Is.Not.Null);
    }

    [Test]
    public async Task Tf2Teamwork()
    {
        var token = RequireToken(Tokens.TeamworkToken, "TeamworkTF");
        Assert.That(await TeamFortressService.GetMapStatsAsync(token, "pl_upward"), Is.Not.Null);
        Assert.That(await TeamFortressService.GetNewsArticlesAsync(token), Is.Not.Null);
        Assert.That(await TeamFortressService.GetServersByGameModeAsync(token, "payload"), Is.Not.Null);
        Assert.That(await TeamFortressService.GetCustomServerListsAsync(token), Is.Not.Null);
    }

    [Test]
    public async Task Twitch()
    {
        var clientId = RequireToken(Tokens.TwitchToken, "Twitch");
        var accessToken = RequireToken(Tokens.TwitchAccess, "TwitchAccess");
        // Offline channels return null, so only check that the request itself works.
        Assert.That(async () => await TwitchService.GetTwitchDataAsync(clientId, accessToken, "rifftrax"), Throws.Nothing);
    }

    [Test]
    public async Task Weather()
    {
        var token = RequireToken(Tokens.WeatherToken, "Weather");
        Assert.That(await WeatherService.GetWeatherDataAsync(token, "Toronto"), Is.Not.Null);
    }

    [Test]
    public async Task Wikipedia()
    {
        Assert.That(await WikipediaService.GetWikipediaArticlesAsync("Russia"), Is.Not.Null);
    }

    [Test]
    public async Task YouTube()
    {
        RequireToken(Tokens.YouTubeToken, "YouTube");
        var service = new YoutubeService();
        Assert.That(await service.GetFirstVideoResultAsync("never gonna give you up"), Does.StartWith("https://www.youtube.com/watch?v="));
        Assert.That(await service.GetEmbeddedResults("linus tech tips", 5, YoutubeService.YouTubeSearch.Channel), Is.Not.Null);
    }
}
