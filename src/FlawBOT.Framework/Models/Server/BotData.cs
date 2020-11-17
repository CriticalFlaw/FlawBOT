using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    public class TokenData
    {
        [JsonProperty("prefix")] public string CommandPrefix { get; private set; }

        [JsonProperty("discord")] public string DiscordToken { get; private set; }

        [JsonProperty("google", NullValueHandling = NullValueHandling.Ignore)]
        public string GoogleToken { get; private set; }

        [JsonProperty("steam", NullValueHandling = NullValueHandling.Ignore)]
        public string SteamToken { get; private set; }

        [JsonProperty("imgur", NullValueHandling = NullValueHandling.Ignore)]
        public string ImgurToken { get; private set; }

        [JsonProperty("omdb", NullValueHandling = NullValueHandling.Ignore)]
        public string OmdbToken { get; private set; }

        [JsonProperty("twitch", NullValueHandling = NullValueHandling.Ignore)]
        public string TwitchToken { get; private set; }

        [JsonProperty("nasa", NullValueHandling = NullValueHandling.Ignore)]
        public string NasaToken { get; private set; }

        [JsonProperty("teamworktf", NullValueHandling = NullValueHandling.Ignore)]
        public string TeamworkToken { get; private set; }

        [JsonProperty("news", NullValueHandling = NullValueHandling.Ignore)]
        public string NewsToken { get; private set; }
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