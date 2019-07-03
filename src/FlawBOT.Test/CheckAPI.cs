using FlawBOT.Services;
using FlawBOT.Services.Search;
using NUnit.Framework;

namespace Tests
{
    public class CheckAPI
    {
        [Test]
        public void Dog()
        {
            Assert.IsTrue(DogService.GetDogPhotoAsync().Result.status == "success");
        }

        [Test]
        public void Frinkiac()
        {
            Assert.NotNull(SimpsonsService.GetSimpsonsDataAsync("frinkiac").Result.Title);
            Assert.NotNull(SimpsonsService.GetSimpsonsDataAsync("morbotron").Result.Title);
            Assert.NotNull(SimpsonsService.GetSimpsonsDataAsync("masterofallscience").Result.Title);
        }

        [Test]
        public void IMDB()
        {
            Assert.Null(IMDBService.GetMovieDataAsync("Office+Space").Result);
        }

        [Test]
        public void SteamUser()
        {
            Assert.NotNull(SteamService.GetSteamUserProfileAsync("criticalflaw"));
            Assert.NotNull(SteamService.GetSteamUserSummaryAsync("criticalflaw"));
        }

        [Test]
        public void Twitch()
        {
            Assert.NotNull(TwitchService.GetTwitchDataAsync("rifftrax"));
        }

        [Test]
        public void UrbanDictionary()
        {
            Assert.IsFalse(DictionaryService.GetDictionaryForTermAsync("computer").Result.result_type == "no_results");
        }

        [Test]
        public void Wikipedia()
        {
            Assert.IsFalse(WikipediaService.GetWikipediaDataAsync("Russia").Result.Query.Pages[0].Missing);
        }
    }
}