using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlawBOT.Framework.Models
{
    #region TEAMWORK_NEWS

    public class TeamworkNews
    {
        [JsonProperty("hash")]
        public string HASH { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }
    }

    #endregion TEAMWORK_NEWS

    #region TEAMWORK_SERVERS

    public class TeamworkServer
    {
        [JsonProperty("ip")]
        public string IP { get; set; }

        [JsonProperty("port")]
        public string Port { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("reachable")]
        public bool Reachable { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("valve_secure")]
        public bool ValveSecure { get; set; }

        [JsonProperty("map_name")]
        public string MapName { get; set; }

        [JsonProperty("map_name_thumbnail")]
        public string MapThumbnail { get; set; }

        [JsonProperty("map_name_next")]
        public string NextMap { get; set; }

        [JsonProperty("players")]
        public int PlayerCount { get; set; }

        [JsonProperty("max_players")]
        public int PlayerMax { get; set; }

        [JsonProperty("gamemodes")]
        public List<string> GameModes { get; set; }

        [JsonProperty("gametype")]
        public string GameType { get; set; }

        [JsonProperty("has_password")]
        public bool? HasPassword { get; set; }

        [JsonProperty("has_rtd")]
        public bool HasRTD { get; set; }

        [JsonProperty("has_randomcrits")]
        public bool? HasRandomCrits { get; set; }

        [JsonProperty("has_norespawntime")]
        public bool HasNoSpawnTimer { get; set; }

        [JsonProperty("has_alltalk")]
        public bool HasAllTalk { get; set; }
    }

    #endregion TEAMWORK_SERVERS

    #region TEAMWORK_MAPS

    public class TeamworkMap
    {
        [JsonProperty("map")]
        public string Map { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("first_seen")]
        public object FirstSeen { get; set; }

        [JsonProperty("last_seen")]
        public string LastSeen { get; set; }

        [JsonProperty("all_gamemodes")]
        public List<string> GameModes { get; set; }

        [JsonProperty("all_server_types")]
        public List<string> ServerTypes { get; set; }

        [JsonProperty("highest_players")]
        public int HighestPlayerCount { get; set; }

        [JsonProperty("highest_servers")]
        public int HighestServerCount { get; set; }

        [JsonProperty("alltime_avg_players")]
        public string AvgPlayers { get; set; }

        [JsonProperty("alltime_avg_players_days")]
        public int AvgPlayersDays { get; set; }

        [JsonProperty("official_map")]
        public bool OfficialMap { get; set; }

        [JsonProperty("normalized_map_name")]
        public string MapName { get; set; }

        [JsonProperty("related_maps")]
        public List<string> RelatedMaps { get; set; }
    }

    #endregion TEAMWORK_MAPS
}