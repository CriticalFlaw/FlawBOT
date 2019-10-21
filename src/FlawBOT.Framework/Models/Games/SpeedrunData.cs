using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FlawBOT.Framework.Models
{
    public class SpeedrunGame
    {
        [JsonProperty("data")]
        public List<Data> Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("names")]
        public Names Names { get; set; }

        [JsonProperty("abbreviation")]
        public string Abbreviation { get; set; }

        [JsonProperty("weblink")]
        public string WebLink { get; set; }

        [JsonProperty("released")]
        public int ReleaseYear { get; set; }

        [JsonProperty("release-date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("ruleset")]
        public Ruleset RuleSet { get; set; }

        [JsonProperty("romhack")]
        public bool ROMHack { get; set; }

        [JsonProperty("gametypes")]
        public List<object> GameTypes { get; set; }

        [JsonProperty("platforms")]
        public List<object> Platforms { get; set; }

        [JsonProperty("regions")]
        public List<string> Regions { get; set; }

        [JsonProperty("genres")]
        public List<object> Genres { get; set; }

        [JsonProperty("engines")]
        public List<object> Engines { get; set; }

        [JsonProperty("developers")]
        public List<object> Developers { get; set; }

        [JsonProperty("publishers")]
        public List<object> Publishers { get; set; }

        [JsonProperty("created")]
        public DateTime? Created { get; set; }

        [JsonProperty("assets")]
        public Assets Assets { get; set; }

        [JsonProperty("links")]
        public List<Link> Links { get; set; }
    }

    public class Names
    {
        [JsonProperty("international")]
        public string International { get; set; }

        [JsonIgnore]
        [JsonProperty("japanese")]
        public string Japanese { get; set; }

        [JsonProperty("twitch")]
        public string Twitch { get; set; }
    }

    public class Ruleset
    {
        [JsonProperty("show-milliseconds")]
        public bool ShowMilliseconds { get; set; }

        [JsonProperty("require-verification")]
        public bool RequiresVerification { get; set; }

        [JsonProperty("require-video")]
        public bool RequiresVideo { get; set; }

        [JsonIgnore]
        [JsonProperty("run-times")]
        public List<string> RunTimes { get; set; }

        [JsonIgnore]
        [JsonProperty("default-time")]
        public string DefaultTime { get; set; }

        [JsonProperty("emulators-allowed")]
        public bool EmulatorsAllowed { get; set; }
    }

    public class Assets
    {
        [JsonProperty("logo")]
        public Image Logo { get; set; }

        [JsonProperty("cover-small")]
        public Image CoverSmall { get; set; }

        [JsonProperty("cover-medium")]
        public Image CoverMedium { get; set; }

        [JsonProperty("cover-large")]
        public Image CoverLarge { get; set; }

        [JsonProperty("icon")]
        public Image Icon { get; set; }

        [JsonProperty("foreground")]
        public object Foreground { get; set; }
    }

    public class Image
    {
        [JsonProperty("uri")]
        public string URL { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Link
    {
        [JsonProperty("rel")]
        public string REL { get; set; }

        [JsonProperty("uri")]
        public string URL { get; set; }
    }

    public class ExtraData
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("released")]
        public int? Released { get; set; }

        [JsonProperty("links")]
        public List<Link> Links { get; set; }
    }

    public class SpeedrunExtra
    {
        [JsonProperty("data")]
        public ExtraData Data { get; set; }
    }

    public enum SpeedrunExtras
    {
        Platforms,
        Genres,
        Developers,
        Publishers
    }
}