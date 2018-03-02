using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class CommonService
    {
        public static List<string> Get8BallList()
        {
            List<string> EightBallAnswers = new List<string>
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
            return EightBallAnswers;
        }
    }

    public class SteamService
    {
        public static async Task<RootObject> GetSteamAppsListAsync()
        {
            using (var HTTP = new HttpClient())
            {
                var result = await HTTP.GetStringAsync("http://api.steampowered.com/ISteamApps/GetAppList/v0002/");
                return JsonConvert.DeserializeObject<RootObject>(result);
            }
        }

        public class App
        {
            public int appid { get; set; }
            public string name { get; set; }
        }

        public class Applist
        {
            public List<App> apps { get; set; }
        }

        public class RootObject
        {
            public Applist applist { get; set; }
        }
    }

    public class SteamGameService
    {
        public static async Task<RootObject> GetSteamAppsInfoAsync(uint appID)
        {
            using (var HTTP = new HttpClient())
            {
                var result = await HTTP.GetStringAsync($"http://store.steampowered.com/api/appdetails/?appids={appID}");
                return JsonConvert.DeserializeObject<RootObject>(result);
            }
        }

        public class PcRequirements
        {
            public string minimum { get; set; }
        }

        public class PriceOverview
        {
            public string currency { get; set; }
            public int initial { get; set; }
            public int final { get; set; }
            public int discount_percent { get; set; }
        }

        public class Sub
        {
            public int packageid { get; set; }
            public string percent_savings_text { get; set; }
            public int percent_savings { get; set; }
            public string option_text { get; set; }
            public string option_description { get; set; }
            public string can_get_free_license { get; set; }
            public bool is_free_license { get; set; }
            public int price_in_cents_with_discount { get; set; }
        }

        public class PackageGroup
        {
            public string name { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string selection_text { get; set; }
            public string save_text { get; set; }
            public int display_type { get; set; }
            public string is_recurring_subscription { get; set; }
            public List<Sub> subs { get; set; }
        }

        public class Platforms
        {
            public bool windows { get; set; }
            public bool mac { get; set; }
            public bool linux { get; set; }
        }

        public class Category
        {
            public int id { get; set; }
            public string description { get; set; }
        }

        public class Genre
        {
            public string id { get; set; }
            public string description { get; set; }
        }

        public class Screenshot
        {
            public int id { get; set; }
            public string path_thumbnail { get; set; }
            public string path_full { get; set; }
        }

        public class Webm
        {
            public string __invalid_name__480 { get; set; }
            public string max { get; set; }
        }

        public class Movie
        {
            public int id { get; set; }
            public string name { get; set; }
            public string thumbnail { get; set; }
            public Webm webm { get; set; }
            public bool highlight { get; set; }
        }

        public class Recommendations
        {
            public int total { get; set; }
        }

        public class Highlighted
        {
            public string name { get; set; }
            public string path { get; set; }
        }

        public class Achievements
        {
            public int total { get; set; }
            public List<Highlighted> highlighted { get; set; }
        }

        public class ReleaseDate
        {
            public bool coming_soon { get; set; }
            public string date { get; set; }
        }

        public class SupportInfo
        {
            public string url { get; set; }
            public string email { get; set; }
        }

        public class Data
        {
            public string type { get; set; }
            public string name { get; set; }
            public int steam_appid { get; set; }
            public int required_age { get; set; }
            public bool is_free { get; set; }
            public List<int> dlc { get; set; }
            public string detailed_description { get; set; }
            public string about_the_game { get; set; }
            public string short_description { get; set; }
            public string supported_languages { get; set; }
            public string header_image { get; set; }
            public string website { get; set; }
            public PcRequirements pc_requirements { get; set; }
            public List<object> mac_requirements { get; set; }
            public List<object> linux_requirements { get; set; }
            public List<string> developers { get; set; }
            public List<string> publishers { get; set; }
            public PriceOverview price_overview { get; set; }
            public List<int> packages { get; set; }
            public List<PackageGroup> package_groups { get; set; }
            public Platforms platforms { get; set; }
            public List<Category> categories { get; set; }
            public List<Genre> genres { get; set; }
            public List<Screenshot> screenshots { get; set; }
            public List<Movie> movies { get; set; }
            public Recommendations recommendations { get; set; }
            public Achievements achievements { get; set; }
            public ReleaseDate release_date { get; set; }
            public SupportInfo support_info { get; set; }
            public string background { get; set; }
        }

        public class __invalid_type__526160
        {
            public bool success { get; set; }
            public Data data { get; set; }
        }

        public class RootObject
        {
            public __invalid_type__526160 __invalid_name__526160 { get; set; }
        }
    }

    public class DefinitionService
    {
        public static async Task<Data> GetDefinitionForTermAsync(string query)
        {
            using (var HTTP = new HttpClient())
            {
                var result = await HTTP.GetStringAsync($"http://api.pearson.com/v2/dictionaries/entries?headword={WebUtility.UrlEncode(query.Trim())}");
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
                var data = await http.GetStringAsync($"http://api.urbandictionary.com/v0/define?term={WebUtility.UrlEncode(query)}");
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
                var data = await http.GetStringAsync($"http://api.pokemontcg.io/v1/cards?name={query}");
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
                var result = await http.GetStringAsync($"https://frinkiac.com/api/random");
                return JsonConvert.DeserializeObject<RootObject>(result);
            }
        }

        public static async Task<string> GetSimpsonsGifAsync()
        {
            using (var http = new HttpClient())
            {
                var result = await http.GetStringAsync($"https://frinkiac.com/api/random");
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
        public StreamInfo stream { get; set; }
        public string Game => stream?.game;
        public int Viewers => stream?.viewers ?? 0;
        public string Title => stream?.channel?.status;
        public string Icon => stream?.channel?.logo;
        public int Followers => stream?.channel?.followers ?? 0;
        public bool IsLive => stream != null;
        public bool Live => IsLive;
        public string Error { get; set; } = null;
        public string Url { get; set; }

        public class StreamInfo
        {
            public string game { get; set; }
            public int viewers { get; set; }
            public ChannelInfo channel { get; set; }

            public class ChannelInfo
            {
                public string display_name { get; set; }
                public string status { get; set; }
                public string logo { get; set; }
                public int followers { get; set; }
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

            //    [JsonProperty("temp_min")]
            public double tempMin { get; set; }

            //    [JsonProperty("temp_max")]
            public double tempMax { get; set; }
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