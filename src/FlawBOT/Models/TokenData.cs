using Newtonsoft.Json;

namespace FlawBOT.Models
{
    public class TokenData
    {
        [JsonProperty("prefix")] public string CommandPrefix { get; set; }

        [JsonProperty("discord")] public string DiscordToken { get; set; }

        [JsonProperty("steam", NullValueHandling = NullValueHandling.Ignore)]
        public string SteamToken { get; set; }

        [JsonProperty("imgur", NullValueHandling = NullValueHandling.Ignore)]
        public string ImgurToken { get; set; }

        [JsonProperty("omdb", NullValueHandling = NullValueHandling.Ignore)]
        public string OmdbToken { get; set; }

        [JsonProperty("twitch", NullValueHandling = NullValueHandling.Ignore)]
        public string TwitchToken { get; set; }

        [JsonProperty("nasa", NullValueHandling = NullValueHandling.Ignore)]
        public string NasaToken { get; set; }

        [JsonProperty("teamworktf", NullValueHandling = NullValueHandling.Ignore)]
        public string TeamworkToken { get; set; }

        [JsonProperty("news", NullValueHandling = NullValueHandling.Ignore)]
        public string NewsToken { get; set; }

        [JsonProperty("weather", NullValueHandling = NullValueHandling.Ignore)]
        public string WeatherToken { get; set; }

        [JsonProperty("youtube", NullValueHandling = NullValueHandling.Ignore)]
        public string YouTubeToken { get; set; }
    }
}