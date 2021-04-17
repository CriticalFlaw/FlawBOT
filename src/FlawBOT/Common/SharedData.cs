using System;
using System.Reflection;
using System.Text.Json.Serialization;
using DSharpPlus.Entities;
using FlawBOT.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Common
{
    public class BotSettings
    {
        public DiscordColor DefaultColor = new("#00FF7F");
        [JsonPropertyName("Name")] public string Name;
        [JsonPropertyName("Prefix")] public string Prefix;
        [JsonPropertyName("ShardCount")] public int ShardCount;
        [JsonPropertyName("Tokens")] public Tokens Tokens;
        [JsonPropertyName("Lavalink")] public bool UseLavalink;
        public string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        public string GitHubLink { get; } = Resources.URL_BOT_GitHub;
        public string InviteLink { get; } = Resources.URL_BOT_Invite;
        public string DocsLink { get; } = Resources.URL_BOT_Docs;
        public DateTime ProcessStarted { get; set; }
    }

    public class Tokens
    {
        [JsonProperty("Discord")] public string DiscordToken { get; set; }

        [JsonProperty("Steam", NullValueHandling = NullValueHandling.Ignore)]
        public string SteamToken { get; set; }

        [JsonProperty("Imgur", NullValueHandling = NullValueHandling.Ignore)]
        public string ImgurToken { get; set; }

        [JsonProperty("OMDB", NullValueHandling = NullValueHandling.Ignore)]
        public string OmdbToken { get; set; }

        [JsonProperty("Twitch", NullValueHandling = NullValueHandling.Ignore)]
        public string TwitchToken { get; set; }

        [JsonProperty("NASA", NullValueHandling = NullValueHandling.Ignore)]
        public string NasaToken { get; set; }

        [JsonProperty("TeamworkTF", NullValueHandling = NullValueHandling.Ignore)]
        public string TeamworkToken { get; set; }

        [JsonProperty("News", NullValueHandling = NullValueHandling.Ignore)]
        public string NewsToken { get; set; }

        [JsonProperty("Weather", NullValueHandling = NullValueHandling.Ignore)]
        public string WeatherToken { get; set; }

        [JsonProperty("YouTube", NullValueHandling = NullValueHandling.Ignore)]
        public string YouTubeToken { get; set; }
    }

    public enum ResponseType
    {
        Default,
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