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

        [JsonProperty("steam64")]
        public string SteamID { get; private set; }

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

        [JsonProperty("backpacktf")]
        public string BackpackToken { get; private set; }

        [JsonProperty("backpacktf_secret")]
        public string BackpackAccess { get; private set; }

        [JsonProperty("backpacktf_schema")]
        public string BackpackSchema { get; private set; }

        [JsonProperty("reddit_appid")]
        public string RedditAppToken { get; private set; }

        [JsonProperty("reddit_access")]
        public string RedditAccessToken { get; private set; }

        [JsonProperty("reddit_refresh")]
        public string RedditRefreshToken { get; private set; }
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