using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    public class TokenData
    {
        [JsonProperty("prefix")]
        public string CommandPrefix { get; private set; }

        [JsonProperty("discord")]
        public string DiscordToken { get; private set; }

        [JsonProperty("google")]
        public string GoogleToken { get; private set; }

        [JsonProperty("steam")]
        public string SteamToken { get; private set; }

        [JsonProperty("imgur")]
        public string ImgurToken { get; private set; }

        [JsonProperty("omdb")]
        public string OMDBToken { get; private set; }

        [JsonProperty("twitch")]
        public string TwitchToken { get; private set; }

        [JsonProperty("nasa")]
        public string NASAToken { get; private set; }

        [JsonProperty("teamworktf")]
        public string TeamworkToken { get; private set; }

        [JsonProperty("news")]
        public string NewsToken { get; private set; }

        [JsonProperty("goodreads")]
        public string GoodReadsToken { get; private set; }
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