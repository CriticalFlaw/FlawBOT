using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    internal class EightBallAnswers
    {
        public static List<string> list = new List<string>
        {
            "It is certain",
            "It is decidedly so",
            "Without a doubt",
            "Yes definitely",
            "You may rely on it",
            "As I see it, yes",
            "Most likely",
            "Outlook good",
            "Yes",
            "Signs point to yes",
            "Reply hazy try again",
            "Ask again later",
            "Better not tell you now",
            "Cannot predict now",
            "Concentrate and ask again",
            "Don't count on it",
            "My reply is no",
            "My sources say no",
            "Outlook not so good",
            "Very doubtful"
        };
    }

    public class DefinitionService
    {
        public static async Task<Data> GetDefinitionForTermAsync(string query)
        {
            using (var http = new HttpClient())
            {
                var result = await http.GetStringAsync($"http://api.pearson.com/v2/dictionaries/entries?headword={WebUtility.UrlEncode(query.Trim())}");
                return JsonConvert.DeserializeObject<Data>(result);
            }
        }

        public class Data
        {
            public List<Result> Results { get; set; }
        }

        public class Result
        {
            public string Part_of_speech { get; set; }
            public List<Sens> Senses { get; set; }
            public string Url { get; set; }
        }

        public class Sens
        {
            public object Definition { get; set; }
            public List<Example> Examples { get; set; }
            public GramaticalInfo Gramatical_info { get; set; }
        }

        public class Example
        {
            public List<Audio> audio { get; set; }
            public string text { get; set; }
        }

        public class GramaticalInfo
        {
            public string type { get; set; }
        }

        public class Audio
        {
            public string url { get; set; }
        }
    }

    public class DictionaryService
    {
        public static async Task<RootObject> GetDictionaryForTermAsync(string query)
        {
            using (var http = new HttpClient())
            {
                var data = await http.GetStringAsync($"http://api.urbandictionary.com/v0/define?term={WebUtility.UrlEncode(query.Trim())}");
                return JsonConvert.DeserializeObject<RootObject>(data);
            }
        }

        public class RootObject
        {
            public string result_type { get; set; }
            public List<List> list { get; set; }
        }

        public class List
        {
            public string definition { get; set; }
            public string author { get; set; }
            public string permalink { get; set; }
            public string example { get; set; }
        }
    }

    public class PokemonService
    {
        public static async Task<RootObject> GetPokemonDataAsync(string query)
        {
            using (var http = new HttpClient())
            {
                var data = await http.GetStringAsync($"http://api.pokemontcg.io/v1/cards?name={query.Trim()}");
                return JsonConvert.DeserializeObject<RootObject>(data);
            }
        }

        public class RootObject
        {
            public List<Card> cards { get; set; }
        }

        public class Card
        {
            public string id { get; set; }
        }
    }

    public class SimpsonsService
    {
        public static async Task<RootObject> GetSimpsonsDataAsync()
        {
            using (var http = new HttpClient())
            {
                var result = await http.GetStringAsync("https://frinkiac.com/api/random");
                return JsonConvert.DeserializeObject<RootObject>(result);
            }
        }

        public static async Task<string> GetSimpsonsGifAsync()
        {
            using (var http = new HttpClient())
            {
                var result = await http.GetStringAsync("https://frinkiac.com/api/random");
                var content = JsonConvert.DeserializeObject<RootObject>(result);
                var frames_result = await http.GetStringAsync($"https://frinkiac.com/api/frames/{content.Episode.Key}/{content.Frame.Timestamp}/3000/4000");
                var frames = JsonConvert.DeserializeObject<List<Frame>>(frames_result);
                var start = frames[0].Timestamp;
                var end = frames[frames.Count - 1].Timestamp;
                return $"https://frinkiac.com/gif/{content.Episode.Key}/{start}/{end}.gif";
            }
        }

        public class RootObject
        {
            public Episode Episode { get; set; }
            public Frame Frame { get; set; }
        }

        public class Episode
        {
            public int Id { get; set; }
            public string Key { get; set; }
            public string Title { get; set; }
            public string Director { get; set; }
            public string Writer { get; set; }
            public string OriginalAirDate { get; set; }
            public string WikiLink { get; set; }
        }

        public class Frame
        {
            public int Id { get; set; }
            public string Episode { get; set; }
            public int Timestamp { get; set; }
        }
    }

    public class SteamService
    {
        public static async Task<RootObject> GetSteamAppsListAsync()
        {
            using (var http = new HttpClient())
            {
                var result = await http.GetStringAsync("http://api.steampowered.com/ISteamApps/GetAppList/v0002/");
                return JsonConvert.DeserializeObject<RootObject>(result);
            }
        }

        public class RootObject
        {
            public Applist applist { get; set; }
        }

        public class Applist
        {
            public List<App> apps { get; set; }
        }

        public class App
        {
            public int appid { get; set; }
            public string name { get; set; }
        }
    }

    public class TimeService
    {
        public GeolocationModel[] results;
        public string status { get; set; }

        public class GeolocationModel
        {
            public string formatted_address { get; set; }
            public GeometryModel geometry { get; set; }

            public class GeometryModel
            {
                public LocationModel location { get; set; }

                public class LocationModel
                {
                    public float lat { get; set; }
                    public float lng { get; set; }
                }
            }
        }

        public class TimeZoneResult
        {
            public double dstOffset { get; set; }
            public double rawOffset { get; set; }
            public string timeZoneName { get; set; }
        }
    }

    public class TwitchService : IStreamResponse
    {
        public StreamInfo Stream { get; set; }
        public bool IsLive => Stream != null;
        public string Error { get; set; } = null;
        public string Game => Stream?.Game;
        public int Viewers => Stream?.Viewers ?? 0;
        public string Title => Stream?.Channel?.Status;
        public string Icon => Stream?.Channel?.Logo;
        public int Followers => Stream?.Channel?.Followers ?? 0;
        public bool Live => IsLive;
        public string Url { get; set; }

        public class StreamInfo
        {
            public string Game { get; set; }
            public int Viewers { get; set; }
            public ChannelInfo Channel { get; set; }

            public class ChannelInfo
            {
                public string display_name { get; set; }
                public string Status { get; set; }
                public string Logo { get; set; }
                public int Followers { get; set; }
            }
        }
    }

    public class WeatherService
    {
        public static double CelsiusToFahrenheit(double cel)
        {
            return cel * 1.8f + 32;
        }

        public class WeatherData
        {
            public int id { get; set; }
            public string name { get; set; }
            public Sys sys { get; set; }
            public Main main { get; set; }
            public Wind wind { get; set; }
            public List<Weather> weather { get; set; }
            public int cod { get; set; }
        }

        public class Sys
        {
            public int id { get; set; }
            public string country { get; set; }
        }

        public class Main
        {
            public double temp { get; set; }
            public float humidity { get; set; }
            public double tempMin { get; set; } // [JsonProperty("temp_min")]
            public double tempMax { get; set; } // [JsonProperty("temp_max")]
        }

        public class Wind
        {
            public double speed { get; set; }
        }

        public class Weather
        {
            public int id { get; set; }
            public string main { get; set; }
        }
    }

    public class WikipediaService
    {
        public WikipediaQuery Query { get; set; }

        public class WikipediaQuery
        {
            public WikipediaPage[] Pages { get; set; }

            public class WikipediaPage
            {
                public bool Missing { get; set; } = false;
                public string FullUrl { get; set; }
            }
        }
    }

    public interface IStreamResponse
    {
        int Viewers { get; }
        string Title { get; }
        bool Live { get; }
        string Game { get; }
        int Followers { get; }
        string Url { get; }
        string Icon { get; }
    }
}