using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    #region MODEL_GAME

    public class SpeedrunGame
    {
        [JsonProperty("data")]
        public List<Datum> Data { get; set; }

        [JsonIgnore]
        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
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

    public class Logo
    {
        [JsonProperty("uri")]
        public string URL { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class CoverTiny
    {
        [JsonProperty("uri")]
        public string URL { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class CoverSmall
    {
        [JsonProperty("uri")]
        public string URL { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class CoverMedium
    {
        [JsonProperty("uri")]
        public string URL { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class CoverLarge
    {
        [JsonProperty("uri")]
        public string URL { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Icon
    {
        [JsonProperty("uri")]
        public string URL { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Trophy1st
    {
        [JsonProperty("uri")]
        public string URL { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Trophy2nd
    {
        [JsonProperty("uri")]
        public string URL { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Trophy3rd
    {
        [JsonProperty("uri")]
        public string URL { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Background
    {
        [JsonProperty("uri")]
        public string URL { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Assets
    {
        [JsonProperty("logo")]
        public Logo Logo { get; set; }

        [JsonProperty("cover-tiny")]
        public CoverTiny CoverTiny { get; set; }

        [JsonProperty("cover-small")]
        public CoverSmall CoverSmall { get; set; }

        [JsonProperty("cover-medium")]
        public CoverMedium CoverMedium { get; set; }

        [JsonProperty("cover-large")]
        public CoverLarge CoverLarge { get; set; }

        [JsonProperty("icon")]
        public Icon Icon { get; set; }

        [JsonIgnore]
        [JsonProperty("trophy-1st")]
        public Trophy1st Trophy1st { get; set; }

        [JsonIgnore]
        [JsonProperty("trophy-2nd")]
        public Trophy2nd Trophy2nd { get; set; }

        [JsonIgnore]
        [JsonProperty("trophy-3rd")]
        public Trophy3rd Trophy3rd { get; set; }

        [JsonProperty("background")]
        public Background Background { get; set; }

        [JsonProperty("foreground")]
        public object Foreground { get; set; }
    }

    public class Datum
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

    public class Link
    {
        [JsonProperty("rel")]
        public string REL { get; set; }

        [JsonProperty("uri")]
        public string URL { get; set; }
    }

    public class Link2
    {
        [JsonProperty("rel")]
        public string REL { get; set; }

        [JsonProperty("uri")]
        public string URL { get; set; }
    }

    public class Pagination
    {
        [JsonIgnore]
        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonIgnore]
        [JsonProperty("max")]
        public int Max { get; set; }

        [JsonIgnore]
        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonIgnore]
        [JsonProperty("links")]
        public List<Link2> Links { get; set; }
    }

    #endregion MODEL_GAME

    #region MODEL_OTHER

    public class PlatformLinks
    {
        [JsonProperty("rel")]
        public string REL { get; set; }

        [JsonProperty("uri")]
        public string URL { get; set; }
    }

    public class Data
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("released")]
        public int? Released { get; set; }

        [JsonProperty("links")]
        public List<PlatformLinks> Links { get; set; }
    }

    public class SpeedrunExtra
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    #endregion MODEL_OTHER

    public enum SpeedrunExtras
    {
        Platforms,
        Genres,
        Developers,
        Publishers
    }
}