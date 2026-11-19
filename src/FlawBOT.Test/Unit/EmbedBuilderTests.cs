using System.ServiceModel.Syndication;
using FlawBOT.Common;
using FlawBOT.Models.Dictionary;
using FlawBOT.Models.Images;
using FlawBOT.Models.Imgur;
using FlawBOT.Models.Misc;
using FlawBOT.Models.NASA;
using FlawBOT.Models.News;
using FlawBOT.Models.Nintendo;
using FlawBOT.Models.OMDB;
using FlawBOT.Models.Weather;
using FlawBOT.Services;
using Google.Apis.YouTube.v3.Data;
using Steam.Models.SteamCommunity;
using Steam.Models.SteamStore;
using SteamWebAPI2.Models.GameEconomy;
using TwitchStream = TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream;

namespace FlawBOT.Test.Unit;

/// <summary>
///     Verifies that API responses are turned into valid embeds, using sample payloads in the shape each API returns.
///     These run offline; the Live tests cover the real endpoints.
/// </summary>
[TestFixture]
[Category("Unit")]
internal class EmbedBuilderTests
{
    private static T Parse<T>(string json) => JsonConvert.DeserializeObject<T>(json);

    [Test]
    public void Dictionary()
    {
        var data = Parse<UrbanDictionaryList>($$"""
            { "list": [ { "word": "computer", "definition": "{{new string('a', 900)}}", "example": "use a [computer]",
              "author": "someone", "permalink": "https://www.urbandictionary.com/define.php?term=computer",
              "written_on": "2010-01-01T00:00:00.000Z", "thumbs_up": 10, "thumbs_down": 2 } ] }
            """);
        var embed = DictionaryService.BuildDefinitionEmbed(data);
        Assert.Multiple(() =>
        {
            Assert.That(embed.Title, Is.EqualTo("computer"));
            // Previously "Take(500) + ..." produced "System.Linq.Enumerable+...", not the definition.
            Assert.That(embed.Fields[0].Value, Has.Length.EqualTo(500).And.EndWith("...").And.StartWith("aaa"));
            Assert.That(embed.Footer.Text, Is.EqualTo("Submitted by: someone on 2010-01-01"));
        });
    }

    [Test]
    public void DictionaryWithoutResults()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DictionaryService.BuildDefinitionEmbed(Parse<UrbanDictionaryList>("""{ "list": [] }""")), Is.Null);
            Assert.That(DictionaryService.BuildDefinitionEmbed(null), Is.Null);
        });
    }

    [Test]
    public void Imgur()
    {
        var data = Parse<ImgurGalleryResponse>("""
            { "success": true, "data": [
              { "title": "nsfw", "link": "https://i.imgur.com/nsfw.jpg", "nsfw": true },
              { "title": "An album", "link": "https://imgur.com/a/abc", "is_album": true, "cover": "xyz" } ] }
            """);
        var embed = ImgurService.BuildGalleryEmbed(data);
        Assert.Multiple(() =>
        {
            // NSFW posts are skipped, and albums use their cover image rather than the album page.
            Assert.That(embed.Image.Url.ToString(), Is.EqualTo("https://i.imgur.com/xyz.jpg"));
            Assert.That(embed.Footer.Text, Is.EqualTo("An album"));
        });
    }

    [Test]
    public void ImgurWithOnlyNsfwResults()
    {
        var data = Parse<ImgurGalleryResponse>("""{ "data": [ { "link": "https://i.imgur.com/nsfw.jpg", "nsfw": true } ] }""");
        Assert.That(ImgurService.BuildGalleryEmbed(data), Is.Null);
    }

    [Test]
    public void Dog()
    {
        Assert.Multiple(() =>
        {
            Assert.That(MiscService.BuildDogEmbed(Parse<DogData>("""{ "status": "success", "message": "https://images.dog.ceo/a.jpg" }"""))?.Image.Url.ToString(), Is.EqualTo("https://images.dog.ceo/a.jpg"));
            Assert.That(MiscService.BuildDogEmbed(Parse<DogData>("""{ "status": "error", "message": "Breed not found" }""")), Is.Null);
            Assert.That(MiscService.BuildDogEmbed(null), Is.Null);
        });
    }

    [Test]
    public void NasaImage()
    {
        var data = Parse<NasaData>($$"""
            { "title": "Galaxy", "explanation": "{{new string('b', 5000)}}", "media_type": "image",
              "url": "https://apod.nasa.gov/sd.jpg", "hdurl": "https://apod.nasa.gov/hd.jpg", "copyright": "Someone" }
            """);
        var embed = NasaService.BuildImageEmbed(data);
        Assert.Multiple(() =>
        {
            Assert.That(embed.Title, Is.EqualTo("Galaxy"));
            Assert.That(embed.Image.Url.ToString(), Is.EqualTo("https://apod.nasa.gov/hd.jpg"));
            // Long explanations previously went into the 2048 character footer and threw.
            Assert.That(embed.Description, Has.Length.EqualTo(EmbedExtensions.DescriptionLimit));
        });
    }

    [Test]
    public void NasaVideo()
    {
        var data = Parse<NasaData>("""{ "title": "Launch", "explanation": "Video", "media_type": "video", "url": "https://www.youtube.com/embed/abc" }""");
        var embed = NasaService.BuildImageEmbed(data);
        Assert.Multiple(() =>
        {
            Assert.That(embed.Image, Is.Null);
            Assert.That(embed.Url.ToString(), Is.EqualTo("https://www.youtube.com/embed/abc"));
        });
    }

    [Test]
    public void News()
    {
        var data = Parse<NewsData>("""
            { "status": "ok", "articles": [
              { "title": "Story", "url": "https://example.com/story", "publishedAt": "2026-01-02T03:04:05Z" },
              { "title": "[Removed]", "url": "https://removed.com" } ] }
            """);
        var embed = NewsService.BuildArticlesEmbed(data);
        Assert.Multiple(() =>
        {
            // A single article is enough; previously fewer than five results reported "No results found."
            Assert.That(embed.Fields, Has.Count.EqualTo(1));
            Assert.That(embed.Fields[0].Value, Is.EqualTo("[Story](https://example.com/story)"));
            Assert.That(NewsService.BuildArticlesEmbed(Parse<NewsData>("""{ "status": "error", "code": "apiKeyInvalid" }""")), Is.Null);
        });
    }

    [Test]
    public void Amiibo()
    {
        var data = Parse<NintendoData>("""
            { "amiibo": [ { "name": "Donkey Kong", "amiiboSeries": "Super Smash Bros.", "gameSeries": "Donkey Kong",
              "image": "https://raw.githubusercontent.com/N3evin/AmiiboAPI/master/images/icon.png", "release": { "na": "2014-11-21", "jp": null } } ] }
            """);
        var embed = NintendoService.BuildAmiiboEmbed(data);
        Assert.Multiple(() =>
        {
            Assert.That(embed.Title, Is.EqualTo("Donkey Kong"));
            Assert.That(embed.Fields.Single(x => x.Name.Contains("flag_us")).Value, Is.EqualTo("2014-11-21"));
            // Missing release dates used to make the whole command fail.
            Assert.That(embed.Fields.Single(x => x.Name.Contains("flag_jp")).Value, Is.EqualTo(EmbedExtensions.Unknown));
        });
    }

    [Test]
    public void Omdb()
    {
        var data = Parse<OmdbData>("""
            { "Title": "Office Space", "Year": "1999", "Rated": "R", "Released": "19 Feb 1999", "Runtime": "89 min",
              "Genre": "Comedy", "Director": "Mike Judge", "Actors": "Ron Livingston", "Plot": "Three company workers...",
              "Poster": "N/A", "imdbRating": "7.6", "imdbID": "tt0151804", "BoxOffice": "N/A", "Response": "True" }
            """);
        var embed = OmdbService.BuildMovieEmbed(data);
        Assert.Multiple(() =>
        {
            Assert.That(embed.Title, Is.EqualTo("Office Space (1999)"));
            Assert.That(embed.Fields.Single(x => x.Name == "Box Office").Value, Is.EqualTo(EmbedExtensions.Unknown));
            Assert.That(embed.Image, Is.Null);
            Assert.That(embed.Url.ToString(), Is.EqualTo("https://www.imdb.com/title/tt0151804/"));
            Assert.That(OmdbService.BuildMovieEmbed(Parse<OmdbData>("""{ "Response": "False", "Error": "Movie not found!" }""")), Is.Null);
        });
    }

    [Test]
    public void Weather()
    {
        var data = Parse<WeatherData>("""
            { "location": { "name": "Toronto", "country": "Canada", "localtime": "2026-01-01 10:00" },
              "current": { "temperature": -5, "humidity": 80, "weather_descriptions": [ "Snow" ] } }
            """);
        var embed = WeatherService.BuildWeatherEmbed(data);
        Assert.Multiple(() =>
        {
            Assert.That(embed.Title, Is.EqualTo("Toronto, Canada"));
            Assert.That(embed.Description, Is.EqualTo("Currently: Snow"));
            Assert.That(embed.Fields[0].Value, Does.Contain("-5.0°C").And.Contain("23.0°F"));
            Assert.That(WeatherService.BuildWeatherEmbed(Parse<WeatherData>("""{ "success": false, "error": { "code": 615 } }""")), Is.Null);
        });
    }

    [Test]
    public void SimpsonsUsesGifWhenFramesAreKnown()
    {
        var data = SimpsonsData();
        var frames = new List<Frame> { new() { Timestamp = 1000 }, new() { Timestamp = 4000 } };
        var embed = SimpsonsService.BuildEpisodeEmbed(SimpsonsService.SiteRoot.Frinkiac, data, frames);
        Assert.Multiple(() =>
        {
            Assert.That(embed.Title, Is.EqualTo("Homer at the Bat (S03E17)"));
            Assert.That(embed.Image.Url.ToString(), Is.EqualTo("https://frinkiac.com/gif/S03E17/1000/4000.gif"));
            Assert.That(embed.Fields.Single(x => x.Name == "Director").Value, Is.EqualTo(EmbedExtensions.Unknown));
        });
    }

    [Test]
    public void SimpsonsFallsBackToStillImage()
    {
        var embed = SimpsonsService.BuildEpisodeEmbed(SimpsonsService.SiteRoot.Morbotron, SimpsonsData(), []);
        Assert.That(embed.Image.Url.ToString(), Is.EqualTo("https://morbotron.com/img/S03E17/2500.jpg"));
    }

    private static SimpsonsData SimpsonsData()
    {
        return new SimpsonsData
        {
            Episode = new Episode { Key = "S03E17", Title = "Homer at the Bat", Writer = "John Swartzwelder", OriginalAirDate = "20-Feb-92", WikiLink = "https://en.wikipedia.org/wiki/Homer_at_the_Bat" },
            Frame = new Frame { Episode = "S03E17", Timestamp = 2500 }
        };
    }

    [Test]
    public void RedditFeed()
    {
        var items = Enumerable.Range(1, 8).Select(i =>
        {
            var item = new SyndicationItem($"Post {i}", "content", new Uri($"https://www.reddit.com/r/dotnet/comments/{i}"));
            item.Authors.Add(new SyndicationPerson { Name = $"/u/user{i}" });
            return item;
        });
        var embed = RedditService.BuildFeedEmbed(new SyndicationFeed(items), "dotnet");
        Assert.Multiple(() =>
        {
            Assert.That(embed.Fields, Has.Count.EqualTo(5));
            Assert.That(embed.Fields[0].Value, Does.Match(@"^\[Post \d\]\(https://www\.reddit\.com/r/dotnet/comments/\d\)$"));
            Assert.That(embed.Footer.Text, Is.EqualTo("Search results for r/dotnet on Reddit"));
            Assert.That(RedditService.BuildFeedEmbed(new SyndicationFeed(), "empty"), Is.Null);
        });
    }

    [Test]
    public void TwitchStream()
    {
        var stream = Parse<TwitchStream>("""
            { "user_login": "rifftrax", "user_name": "RiffTrax", "game_name": "Movies", "type": "live", "title": "Riffing",
              "viewer_count": 1234, "started_at": "2026-01-01T20:00:00Z",
              "thumbnail_url": "https://static-cdn.jtvnw.net/previews-ttv/live_user_rifftrax-{width}x{height}.jpg" }
            """);
        var embed = TwitchService.BuildStreamEmbed(stream);
        Assert.Multiple(() =>
        {
            // Previously showed the stream type ("LIVE") as the broadcaster and left the size placeholders in the image link.
            Assert.That(embed.Fields.Single(x => x.Name == "Broadcaster").Value, Is.EqualTo("RiffTrax"));
            Assert.That(embed.Image.Url.ToString(), Is.EqualTo("https://static-cdn.jtvnw.net/previews-ttv/live_user_rifftrax-1280x720.jpg"));
            Assert.That(embed.Url.ToString(), Is.EqualTo("https://www.twitch.tv/rifftrax"));
        });
    }

    [Test]
    public void YouTubeResults()
    {
        var results = new List<SearchResult>
        {
            new() { Id = new ResourceId { Kind = "youtube#video", VideoId = "abc" }, Snippet = new SearchResultSnippet { Title = "Rock &amp; Roll" } },
            new() { Id = new ResourceId { Kind = "youtube#channel", ChannelId = "UC1" }, Snippet = new SearchResultSnippet { Title = "A Channel" } },
            new() { Id = new ResourceId { Kind = "youtube#somethingNew" }, Snippet = new SearchResultSnippet { Title = "Ignored" } }
        };
        var embed = YoutubeService.BuildResultsEmbed(results);
        Assert.Multiple(() =>
        {
            // Unknown result kinds used to discard the whole result set.
            Assert.That(embed.Fields, Has.Count.EqualTo(2));
            Assert.That(embed.Fields[0].Name, Is.EqualTo("Rock & Roll"));
            Assert.That(embed.Fields[0].Value, Is.EqualTo("https://www.youtube.com/watch?v=abc"));
            Assert.That(embed.Fields[1].Value, Is.EqualTo("https://www.youtube.com/channel/UC1"));
            Assert.That(YoutubeService.BuildResultsEmbed(null), Is.Null);
        });
    }

    [Test]
    public void SteamGame()
    {
        var game = new StoreAppDetailsDataModel
        {
            Name = "Team Fortress 2",
            SteamAppId = 440,
            IsFree = true,
            ShortDescription = "<b>Nine</b> distinct classes &amp; more.",
            Developers = ["Valve"],
            Publishers = ["Valve"],
            Genres = [new StoreGenreModel { Description = "Action" }, new StoreGenreModel { Description = "Free to Play" }]
        };
        var embed = SteamService.BuildGameEmbed(game);
        Assert.Multiple(() =>
        {
            Assert.That(embed.Description, Is.EqualTo("Nine distinct classes & more."));
            Assert.That(embed.Fields.Single(x => x.Name == "Price").Value, Is.EqualTo("Free"));
            // Missing data (no release date, no Metacritic score) shouldn't break the embed.
            Assert.That(embed.Fields.Single(x => x.Name == "Metacritic").Value, Is.EqualTo(EmbedExtensions.Unknown));
            Assert.That(embed.Fields.Single(x => x.Name == "Genres").Value, Is.EqualTo("Action, Free to Play"));
            Assert.That(embed.Url.ToString(), Is.EqualTo("https://store.steampowered.com/app/440"));
        });
    }

    [Test]
    public void SteamProfile()
    {
        var profile = new PlayerSummaryModel { Nickname = "CriticalFlaw", SteamId = 76561197960287930, UserStatus = UserStatus.Online, PlayingGameName = "Team Fortress 2" };
        var embed = SteamService.BuildProfileEmbed(profile);
        Assert.Multiple(() =>
        {
            Assert.That(embed.Title, Is.EqualTo("CriticalFlaw"));
            Assert.That(embed.Fields.Single(x => x.Name == "Country").Value, Is.EqualTo(EmbedExtensions.Unknown));
            Assert.That(embed.Fields.Single(x => x.Name == "Now Playing").Value, Is.EqualTo("`Team Fortress 2`"));
            Assert.That(SteamService.BuildProfileEmbed(null), Is.Null);
        });
    }

    [Test]
    public void Tf2SchemaItem()
    {
        var item = new SchemaItem { ItemName = "Force-A-Nature", DefIndex = 45, ItemSlot = "primary", UsedByClasses = ["Scout"], Capabilities = new SchemaCapabilities { Nameable = true } };
        var embed = TeamFortressService.BuildSchemaItemEmbed(item);
        Assert.Multiple(() =>
        {
            Assert.That(embed.Fields.Single(x => x.Name == "Item Slot:").Value, Is.EqualTo("Primary"));
            Assert.That(embed.Fields.Single(x => x.Name == "Used by:").Value, Is.EqualTo("Scout"));
            Assert.That(embed.Url.ToString(), Is.EqualTo("https://wiki.teamfortress.com/wiki/Force-A-Nature"));
            // Items without a slot or capabilities used to throw.
            Assert.That(() => TeamFortressService.BuildSchemaItemEmbed(new SchemaItem { ItemName = "Thing" }), Throws.Nothing);
        });
    }

    [Test]
    public void Tf2SchemaLookupWithoutSchemaReturnsNull()
    {
        Assert.That(TeamFortressService.GetSchemaItem("scattergun"), Is.Null);
    }
}
