using Newtonsoft.Json;
namespace FlawBOT.Models.Speedrun
{
    public class Data
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("names")]
        public Names Names { get; set; }

        [JsonProperty("abbreviation")]
        public string Abbreviation { get; set; }

        [JsonProperty("weblink")]
        public string WebLink { get; set; }

        [JsonProperty("release-date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("platforms")]
        public List<object> Platforms { get; set; }

        [JsonProperty("developers")]
        public List<object> Developers { get; set; }

        [JsonProperty("publishers")]
        public List<object> Publishers { get; set; }

        [JsonProperty("assets")]
        public Assets Assets { get; set; }

        [JsonProperty("links")]
        public List<Link> Links { get; set; }

        [JsonIgnore]
        [JsonProperty("released")]
        public int ReleaseYear { get; set; }

        [JsonIgnore]
        [JsonProperty("ruleset")]
        public int RuleSet { get; set; }

        [JsonIgnore]
        [JsonProperty("romhack")]
        public int RomHack { get; set; }

        [JsonIgnore]
        [JsonProperty("gametypes")]
        public int GameTypes { get; set; }

        [JsonIgnore]
        [JsonProperty("regions")]
        public int Regions { get; set; }

        [JsonIgnore]
        [JsonProperty("genres")]
        public int Genres { get; set; }

        [JsonIgnore]
        [JsonProperty("engines")]
        public int Engines { get; set; }

        [JsonIgnore]
        [JsonProperty("created")]
        public int Created { get; set; }
    }
}