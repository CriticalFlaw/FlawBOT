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

        [JsonProperty("created_at")]
        public CreatedAt created_at { get; set; }
    }

    public class CreatedAt
    {
        [JsonProperty("date")]
        public string date { get; set; }

        [JsonProperty("timezone_type")]
        public int timezone_type { get; set; }

        [JsonProperty("timezone")]
        public string timezone { get; set; }
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

        [JsonProperty("sourcecbl_secure")]
        public bool sourcecbl_secure { get; set; }

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

        [JsonProperty("context")]
        public Context context { get; set; }
    }

    public class Context
    {
        [JsonProperty("normalized_map_name")]
        public string normalized_map_name { get; set; }

        [JsonProperty("file_hash")]
        public string file_hash { get; set; }

        [JsonProperty("map_version_sampled")]
        public int map_version_sampled { get; set; }

        [JsonProperty("entity_count")]
        public int entity_count { get; set; }

        [JsonProperty("level_overview")]
        public LevelOverview level_overview { get; set; }

        [JsonProperty("screenshots")]
        public List<string> screenshots { get; set; }

        [JsonProperty("elo_rating_best")]
        public int elo_rating_best { get; set; }
    }

    public class Context2
    {
        [JsonProperty("screenHeight")]
        public int screenHeight { get; set; }

        [JsonProperty("scale")]
        public int scale { get; set; }

        [JsonProperty("screenWidth")]
        public int screenWidth { get; set; }

        [JsonProperty("y")]
        public int? y { get; set; }

        [JsonProperty("x")]
        public int? x { get; set; }

        [JsonProperty("z")]
        public int? z { get; set; }
    }

    public class LevelOverview
    {
        [JsonProperty("image")]
        public string image { get; set; }

        [JsonProperty("context")]
        public List<Context2> context { get; set; }
    }

    #endregion TEAMWORK_MAPS
}