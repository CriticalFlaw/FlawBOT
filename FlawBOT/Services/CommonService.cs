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

    public class DefinitionService
    {
        public static async Task<Data> GetDefinitionForTermAsync(string query)
        {
            using (var HTTP = new HttpClient())
            {
                var result = await HTTP.GetStringAsync($"http://api.pearson.com/v2/dictionaries/entries?headword=" + WebUtility.UrlEncode(query.Trim())).ConfigureAwait(false);
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
        public static async Task<Data> GetDictionaryForTermAsync(string query)
        {
            using (var http = new HttpClient())
            {
                var result = await http.GetStringAsync($"http://api.urbandictionary.com/v0/define?term={WebUtility.UrlEncode(query)}");
                return JsonConvert.DeserializeObject<Data>(result);
            }
        }

        public class Data
        {
            [JsonProperty("tags")]
            public string[] Tags { get; set; }

            [JsonProperty("result_type")]
            public string ResultType { get; set; }

            [JsonProperty("list")]
            public UrbanDictList[] List { get; set; }
        }

        public class UrbanDictList
        {
            [JsonProperty("definition")]
            public string Definition { get; set; }

            [JsonProperty("permalink")]
            public string Permalink { get; set; }

            [JsonProperty("author")]
            public string Author { get; set; }

            [JsonProperty("word")]
            public string Word { get; set; }

            [JsonProperty("example")]
            public string Example { get; set; }
        }
    }

    public class PokemonService
    {
        public static async Task<RootObject> GetPokemonDataAsync(string query)
        {
            using (var http = new HttpClient())
            {
                var result = await http.GetStringAsync($"http://api.pokemontcg.io/v1/cards?name={query}");
                return JsonConvert.DeserializeObject<RootObject>(result);
            }
        }

        public class RootObject
        {
            public List<Card> cards { get; set; }
        }

        public class Card
        {
            public string id { get; set; }
            public string name { get; set; }
            public int nationalPokedexNumber { get; set; }
            public string imageUrl { get; set; }
            public string imageUrlHiRes { get; set; }
            public List<string> types { get; set; }
            public string supertype { get; set; }
            public string subtype { get; set; }
            public string hp { get; set; }
            public List<string> retreatCost { get; set; }
            public string number { get; set; }
            public string artist { get; set; }
            public string rarity { get; set; }
            public string series { get; set; }
            public string set { get; set; }
            public string setCode { get; set; }
            public List<Attack> attacks { get; set; }
            public List<Resistance> resistances { get; set; }
            public List<Weakness> weaknesses { get; set; }
            public Ability ability { get; set; }
            public string evolvesFrom { get; set; }
            public List<string> text { get; set; }
        }

        public class Attack
        {
            public List<string> cost { get; set; }
            public string name { get; set; }
            public string text { get; set; }
            public string damage { get; set; }
            public int convertedEnergyCost { get; set; }
        }

        public class Resistance
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Weakness
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Ability
        {
            public string name { get; set; }
            public string text { get; set; }
            public string type { get; set; }
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
                //var gif_url = await get_gif_url(content.Episode.Key, content.Frame.Timestamp);
                var frames_result = await http.GetStringAsync($"https://frinkiac.com/api/frames/{content.Episode.Key}/{content.Frame.Timestamp}/3000/4000");
                var frames = JsonConvert.DeserializeObject<List<Frame>>(frames_result);
                var start = frames[0].Timestamp;
                var end = frames[frames.Count - 1].Timestamp;
                return $"https://frinkiac.com/gif/{content.Episode.Key}/{start}/{end}.gif";
            }
        }

        //public static async Task<RootObject> generate_gif(string gif_url)
        //{
        //    using (var http = new HttpClient())
        //    {
        //        var result = await http.GetStringAsync(gif_url);
        //        return JsonConvert.DeserializeObject<RootObject>(result);
        //    }
        //}

        public class RootObject
        {
            public Episode Episode { get; set; }
            public Frame Frame { get; set; }
            public List<Subtitle> Subtitles { get; set; }
            public List<Nearby> Nearby { get; set; }
        }

        public class Episode
        {
            public int Id { get; set; }
            public string Key { get; set; }
            public int Season { get; set; }
            public int EpisodeNumber { get; set; }
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

        public class Subtitle
        {
            public int Id { get; set; }
            public int RepresentativeTimestamp { get; set; }
            public string Episode { get; set; }
            public int StartTimestamp { get; set; }
            public int EndTimestamp { get; set; }
            public string Content { get; set; }
            public string Language { get; set; }
        }

        public class Nearby
        {
            public int Id { get; set; }
            public string Episode { get; set; }
            public int Timestamp { get; set; }
        }
    }

    public class TimeService
    {
        public GeolocationModel[] results;

        public class GeolocationModel
        {
            public GeometryModel Geometry { get; set; }

            public class GeometryModel
            {
                public LocationModel Location { get; set; }

                public class LocationModel
                {
                    public float Lat { get; set; }
                    public float Lng { get; set; }
                }
            }
        }

        public class TimeZoneResult
        {
            public double DstOffset { get; set; }
            public double RawOffset { get; set; }
            public string TimeZoneName { get; set; }
        }
    }

    public class TwitterService
    {
        public RootObject Query { get; set; }

        public class RootObject
        {
            public List<Status> statuses { get; set; }
            public SearchMetadata search_metadata { get; set; }
        }

        public class Status
        {
            public object coordinates { get; set; }
            public bool favorited { get; set; }
            public bool truncated { get; set; }
            public string created_at { get; set; }
            public string id_str { get; set; }
            public Entities entities { get; set; }
            public object in_reply_to_user_id_str { get; set; }
            public object contributors { get; set; }
            public string text { get; set; }
            public Metadata metadata { get; set; }
            public int retweet_count { get; set; }
            public object in_reply_to_status_id_str { get; set; }
            public object id { get; set; }
            public object geo { get; set; }
            public bool retweeted { get; set; }
            public object in_reply_to_user_id { get; set; }
            public object place { get; set; }
            public User user { get; set; }
            public object in_reply_to_screen_name { get; set; }
            public string source { get; set; }
            public object in_reply_to_status_id { get; set; }
        }

        public class Entities
        {
            public List<object> urls { get; set; }
            public List<Hashtag> hashtags { get; set; }
            public List<object> user_mentions { get; set; }
        }

        public class Metadata
        {
            public string iso_language_code { get; set; }
            public string result_type { get; set; }
        }

        public class User
        {
            public string profile_sidebar_fill_color { get; set; }
            public string profile_sidebar_border_color { get; set; }
            public bool profile_background_tile { get; set; }
            public string name { get; set; }
            public string profile_image_url { get; set; }
            public string created_at { get; set; }
            public string location { get; set; }
            public object follow_request_sent { get; set; }
            public string profile_link_color { get; set; }
            public bool is_translator { get; set; }
            public string id_str { get; set; }
            public Entities2 entities { get; set; }
            public bool default_profile { get; set; }
            public bool contributors_enabled { get; set; }
            public int favourites_count { get; set; }
            public string url { get; set; }
            public string profile_image_url_https { get; set; }
            public int utc_offset { get; set; }
            public int id { get; set; }
            public bool profile_use_background_image { get; set; }
            public int listed_count { get; set; }
            public string profile_text_color { get; set; }
            public string lang { get; set; }
            public int followers_count { get; set; }
            public bool @protected { get; set; }
            public object notifications { get; set; }
            public string profile_background_image_url_https { get; set; }
            public string profile_background_color { get; set; }
            public bool verified { get; set; }
            public bool geo_enabled { get; set; }
            public string time_zone { get; set; }
            public string description { get; set; }
            public bool default_profile_image { get; set; }
            public string profile_background_image_url { get; set; }
            public int statuses_count { get; set; }
            public int friends_count { get; set; }
            public object following { get; set; }
            public bool show_all_inline_media { get; set; }
            public string screen_name { get; set; }
        }

        public class Hashtag
        {
            public string text { get; set; }
            public List<int> indices { get; set; }
        }

        public class Entities2
        {
            public Url url { get; set; }
            public Description description { get; set; }
        }

        public class Url
        {
            public List<Url2> urls { get; set; }
        }

        public class Url2
        {
            public object expanded_url { get; set; }
            public string url { get; set; }
            public List<int> indices { get; set; }
        }

        public class Description
        {
            public List<object> urls { get; set; }
        }

        public class SearchMetadata
        {
            public long max_id { get; set; }
            public long since_id { get; set; }
            public string refresh_url { get; set; }
            public string next_results { get; set; }
            public int count { get; set; }
            public double completed_in { get; set; }
            public string since_id_str { get; set; }
            public string query { get; set; }
            public string max_id_str { get; set; }
        }
    }

    public class TwitchService : IStreamResponse
    {
        public string Error { get; set; } = null;
        public bool IsLive => Stream != null;
        public StreamInfo Stream { get; set; }
        public int Viewers => Stream?.Viewers ?? 0;
        public string Title => Stream?.Channel?.Status;
        public bool Live => IsLive;
        public string Game => Stream?.Game;
        public int Followers => Stream?.Channel?.Followers ?? 0;
        public string Url { get; set; }
        public string Icon => Stream?.Channel?.Logo;

        public class StreamInfo
        {
            public int Viewers { get; set; }
            public string Game { get; set; }
            public ChannelInfo Channel { get; set; }

            public class ChannelInfo
            {
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
            public int Id { get; set; }
            public string Name { get; set; }
            public Sys Sys { get; set; }
            public Main Main { get; set; }
            public List<Weather> Weather { get; set; }
            public Wind Wind { get; set; }
        }

        public class Sys
        {
            public int Type { get; set; }
            public int Id { get; set; }
            public double Message { get; set; }
            public string Country { get; set; }
            public double Sunrise { get; set; }
            public double Sunset { get; set; }
        }

        public class Main
        {
            public double Temp { get; set; }
            public float Pressure { get; set; }
            public float Humidity { get; set; }

            [JsonProperty("temp_min")]
            public double TempMin { get; set; }

            [JsonProperty("temp_max")]
            public double TempMax { get; set; }
        }

        public class Weather
        {
            public int Id { get; set; }
            public string Main { get; set; }
            public string Description { get; set; }
            public string Icon { get; set; }
        }

        public class Wind
        {
            public double Speed { get; set; }
            public double Deg { get; set; }
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