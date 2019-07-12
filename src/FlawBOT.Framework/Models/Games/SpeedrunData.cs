using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FlawBOT.Framework.Models
{
    #region MODEL_GAME

    public class SpeedrunGame
    {
        [JsonProperty("data")]
        public List<Datum> Data { get; set; }

        [JsonProperty("pagination")]
        [JsonIgnore]
        public Pagination Pagination { get; set; }
    }

    public class Names
    {
        [JsonProperty("international")]
        public string International { get; set; }

        [JsonProperty("japanese")]
        [JsonIgnore]
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

        [JsonProperty("run-times")]
        [JsonIgnore]
        public List<string> RunTimes { get; set; }

        [JsonProperty("default-time")]
        [JsonIgnore]
        public string DefaultTime { get; set; }

        [JsonProperty("emulators-allowed")]
        public bool EmulatorsAllowed { get; set; }
    }

    public class Moderators
    {
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

        [JsonProperty("trophy-1st")]
        [JsonIgnore]
        public Trophy1st Trophy1st { get; set; }

        [JsonProperty("trophy-2nd")]
        [JsonIgnore]
        public Trophy2nd Trophy2nd { get; set; }

        [JsonProperty("trophy-3rd")]
        [JsonIgnore]
        public Trophy3rd Trophy3rd { get; set; }

        [JsonProperty("trophy-4th")]
        [JsonIgnore]
        public object Trophy4th { get; set; }

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
        public List<string> Platforms { get; set; }

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

        [JsonProperty("moderators")]
        [JsonIgnore]
        public Moderators Moderators { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

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
        [JsonProperty("offset")]
        [JsonIgnore]
        public int Offset { get; set; }

        [JsonProperty("max")]
        [JsonIgnore]
        public int Max { get; set; }

        [JsonProperty("size")]
        [JsonIgnore]
        public int Size { get; set; }

        [JsonProperty("links")]
        [JsonIgnore]
        public List<Link2> Links { get; set; }
    }

    #endregion MODEL_GAME

    #region MODEL_PLATFORM

    public class PlatformLinks
    {
        [JsonProperty("rel")]
        public string REL { get; set; }

        [JsonProperty("uri")]
        public string URL { get; set; }
    }

    public class PlatformData
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("released")]
        public int Released { get; set; }

        [JsonProperty("links")]
        public List<PlatformLinks> Links { get; set; }
    }

    public class SpeedrunPlatform
    {
        [JsonProperty("data")]
        public PlatformData Data { get; set; }
    }

    #endregion MODEL_PLATFORM
}