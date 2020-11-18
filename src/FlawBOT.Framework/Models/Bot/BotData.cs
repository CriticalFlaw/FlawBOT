using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    public class TokenData
    {
        [JsonProperty("prefix")] public string CommandPrefix { get; }

        [JsonProperty("discord")] public string DiscordToken { get; }

        [JsonProperty("steam", NullValueHandling = NullValueHandling.Ignore)]
        public string SteamToken { get; }

        [JsonProperty("imgur", NullValueHandling = NullValueHandling.Ignore)]
        public string ImgurToken { get; }

        [JsonProperty("omdb", NullValueHandling = NullValueHandling.Ignore)]
        public string OmdbToken { get; }

        [JsonProperty("twitch", NullValueHandling = NullValueHandling.Ignore)]
        public string TwitchToken { get; }

        [JsonProperty("nasa", NullValueHandling = NullValueHandling.Ignore)]
        public string NasaToken { get; }

        [JsonProperty("teamworktf", NullValueHandling = NullValueHandling.Ignore)]
        public string TeamworkToken { get; }

        [JsonProperty("news", NullValueHandling = NullValueHandling.Ignore)]
        public string NewsToken { get; }

        [JsonProperty("weather", NullValueHandling = NullValueHandling.Ignore)]
        public string WeatherToken { get; }

        [JsonProperty("youtube", NullValueHandling = NullValueHandling.Ignore)]
        public string YouTubeToken { get; }
    }

    public enum EmbedType
    {
        Default,
        Good,
        Warning,
        Missing,
        Error
    }

    public enum UserStateChange
    {
        Ban,
        Deafen,
        Kick,
        Mute,
        Unban,
        Undeafen,
        Unmute
    }
}