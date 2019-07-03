using Newtonsoft.Json;

namespace FlawBOT.Models
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

        [JsonProperty("steam64")]
        public string SteamID { get; private set; }

        [JsonProperty("imgur")]
        public string ImgurToken { get; private set; }

        [JsonProperty("omdb")]
        public string OMDBToken { get; private set; }

        [JsonProperty("twitch")]
        public string TwitchToken { get; private set; }

        [JsonProperty("teamworktf")]
        public string TeamworkToken { get; private set; }

        [JsonProperty("backpacktf")]
        public string BackpackToken { get; private set; }

        [JsonProperty("backpacktf_secret")]
        public string BackpackAccess { get; private set; }

        [JsonProperty("reddit")]
        public string RedditToken { get; private set; }

        [JsonProperty("reddit_secret")]
        public string RedditSecretToken { get; private set; }
    }

    public enum EmbedType
    {
        Default,
        Good,
        Warning,
        Missing,
        Error
    }
}