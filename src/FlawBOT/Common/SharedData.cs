using DSharpPlus.Entities;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace FlawBOT.Common
{
    public class BotSettings
    {
        [JsonIgnore] public DiscordColor DefaultColor { get; } = new("#00FF7F");
        [JsonProperty("Lavalink")] public Lavalink Lavalink { get; set; } = new();
        [JsonProperty("Name")] public string Name { get; set; } = "FlawBOT";
        [JsonProperty("ShardCount")] public int ShardCount { get; set; } = 1;
        [JsonProperty("Tokens")] public Tokens Tokens { get; set; } = new();
        [JsonIgnore] public string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);
        [JsonIgnore] public string GitHubLink { get; } = Resources.URL_BOT_GitHub;
        [JsonIgnore] public string InviteLink { get; } = Resources.URL_BOT_Invite;
        [JsonIgnore] public string DocsLink { get; } = Resources.URL_BOT_Docs;
        [JsonIgnore] public DateTime ProcessStarted { get; set; } = DateTime.Now;
    }

    public class Lavalink
    {
        [JsonProperty("Enabled")] public bool Enabled { get; set; }
        [JsonProperty("Address")] public string Address { get; set; } = "127.0.0.1";
        [JsonProperty("Port")] public int Port { get; set; } = 2333;
        [JsonProperty("Password")] public string Password { get; set; } = "youshallnotpass";
    }

    public class Tokens
    {
        [JsonProperty("Discord")] public string DiscordToken { get; set; }
        [JsonProperty("Steam")] public string SteamToken { get; set; }
        [JsonProperty("Imgur")] public string ImgurToken { get; set; }
        [JsonProperty("Pokemon")] public string PokemonToken { get; set; }
        [JsonProperty("OMDB")] public string OmdbToken { get; set; }
        [JsonProperty("Twitch")] public string TwitchToken { get; set; }
        [JsonProperty("TwitchAccess")] public string TwitchAccess { get; set; }
        [JsonProperty("NASA")] public string NasaToken { get; set; }
        [JsonProperty("TeamworkTF")] public string TeamworkToken { get; set; }
        [JsonProperty("News")] public string NewsToken { get; set; }
        [JsonProperty("Weather")] public string WeatherToken { get; set; }
        [JsonProperty("YouTube")] public string YouTubeToken { get; set; }
    }

    public enum ResponseType
    {
        Default,
        Warning,
        Missing,
        Error
    }
}
