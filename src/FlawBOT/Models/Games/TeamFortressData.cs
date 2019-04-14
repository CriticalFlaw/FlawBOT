using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlawBOT.Models
{
    #region TEAMWORK_NEWS

    public class TeamworkNews
    {
        [JsonProperty("hash")]
        public string hash { get; set; }

        [JsonProperty("provider")]
        public string provider { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("link")]
        public string link { get; set; }
    }

    #endregion TEAMWORK_NEWS

    #region TEAMWORK_SERVERS

    public class TeamworkServer
    {
        [JsonProperty("ip")]
        public string ip { get; set; }

        [JsonProperty("port")]
        public string port { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("reachable")]
        public bool reachable { get; set; }

        [JsonProperty("provider")]
        public string provider { get; set; }

        [JsonProperty("valve_secure")]
        public bool valve_secure { get; set; }

        [JsonProperty("map_name")]
        public string map_name { get; set; }

        [JsonProperty("map_name_thumbnail")]
        public string map_name_thumbnail { get; set; }

        [JsonProperty("map_name_next")]
        public string map_name_next { get; set; }

        [JsonProperty("players")]
        public int players { get; set; }

        [JsonProperty("max_players")]
        public int max_players { get; set; }

        [JsonProperty("gamemodes")]
        public List<string> gamemodes { get; set; }

        [JsonProperty("gametype")]
        public string gametype { get; set; }

        [JsonProperty("has_password")]
        public bool? has_password { get; set; }

        [JsonProperty("has_rtd")]
        public bool has_rtd { get; set; }

        [JsonProperty("has_randomcrits")]
        public bool? has_randomcrits { get; set; }

        [JsonProperty("has_norespawntime")]
        public bool has_norespawntime { get; set; }

        [JsonProperty("has_alltalk")]
        public bool has_alltalk { get; set; }
    }

    #endregion TEAMWORK_SERVERS

    #region TEAMWORK_MAPS

    public class TeamworkMap
    {
        [JsonProperty("map")]
        public string map { get; set; }

        [JsonProperty("thumbnail")]
        public string thumbnail { get; set; }

        [JsonProperty("first_seen")]
        public object first_seen { get; set; }

        [JsonProperty("last_seen")]
        public string last_seen { get; set; }

        [JsonProperty("all_gamemodes")]
        public List<string> all_gamemodes { get; set; }

        [JsonProperty("all_server_types")]
        public List<string> all_server_types { get; set; }

        [JsonProperty("highest_players")]
        public int highest_players { get; set; }

        [JsonProperty("highest_servers")]
        public int highest_servers { get; set; }

        [JsonProperty("alltime_avg_players")]
        public string alltime_avg_players { get; set; }

        [JsonProperty("alltime_avg_players_days")]
        public int alltime_avg_players_days { get; set; }

        [JsonProperty("official_map")]
        public bool official_map { get; set; }

        [JsonProperty("normalized_map_name")]
        public string normalized_map_name { get; set; }

        [JsonProperty("related_maps")]
        public List<string> related_maps { get; set; }
    }

    #endregion TEAMWORK_MAPS
}