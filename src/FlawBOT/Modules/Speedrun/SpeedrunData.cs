using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FlawBOT.Modules
{
    #region GAME

    public class SpeedrunGame
    {
        [JsonProperty("data")] public List<Data> Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("names")] public Names Names { get; set; }

        [JsonProperty("abbreviation")] public string Abbreviation { get; set; }

        [JsonProperty("weblink")] public string WebLink { get; set; }

        [JsonProperty("release-date")] public string ReleaseDate { get; set; }

        [JsonProperty("platforms")] public List<object> Platforms { get; set; }

        [JsonProperty("developers")] public List<object> Developers { get; set; }

        [JsonProperty("publishers")] public List<object> Publishers { get; set; }

        [JsonProperty("assets")] public Assets Assets { get; set; }

        [JsonProperty("links")] public List<Link> Links { get; set; }

        [JsonIgnore]
        [JsonProperty("released")]
        public int ReleaseYear { get; set; }

        [JsonIgnore][JsonProperty("ruleset")] public int RuleSet { get; set; }

        [JsonIgnore][JsonProperty("romhack")] public int RomHack { get; set; }

        [JsonIgnore]
        [JsonProperty("gametypes")]
        public int GameTypes { get; set; }

        [JsonIgnore][JsonProperty("regions")] public int Regions { get; set; }

        [JsonIgnore][JsonProperty("genres")] public int Genres { get; set; }

        [JsonIgnore][JsonProperty("engines")] public int Engines { get; set; }

        [JsonIgnore][JsonProperty("created")] public int Created { get; set; }
    }

    public class Names
    {
        [JsonProperty("international")] public string International { get; set; }

        [JsonIgnore]
        [JsonProperty("japanese")]
        public string Japanese { get; set; }

        [JsonIgnore][JsonProperty("twitch")] public string Twitch { get; set; }
    }

    public class Assets
    {
        [JsonProperty("logo")] public Image Logo { get; set; }

        [JsonProperty("cover-small")] public Image CoverSmall { get; set; }

        [JsonProperty("cover-medium")] public Image CoverMedium { get; set; }

        [JsonProperty("cover-large")] public Image CoverLarge { get; set; }

        [JsonProperty("icon")] public Image Icon { get; set; }

        [JsonProperty("foreground")] public object Foreground { get; set; }
    }

    public class Image
    {
        [JsonProperty("uri")] public string Url { get; set; }

        [JsonIgnore][JsonProperty("width")] public int Width { get; set; }

        [JsonIgnore][JsonProperty("height")] public int Height { get; set; }
    }

    public class Link
    {
        [JsonProperty("rel")] public string Rel { get; set; }

        [JsonProperty("uri")] public string Url { get; set; }
    }

    #endregion GAME

    #region CATEGORY

    public class SpeedrunCategory
    {
        [JsonProperty("data")] public List<CategoryData> Data { get; set; }
    }

    public class CategoryData
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("weblink")] public Uri Weblink { get; set; }

        [JsonIgnore][JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("rules")] public string Rules { get; set; }

        [JsonProperty("players")] public Players Players { get; set; }

        [JsonIgnore]
        [JsonProperty("miscellaneous")]
        public bool Miscellaneous { get; set; }

        [JsonProperty("links")] public List<Link> Links { get; set; }
    }

    public class Players
    {
        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("value")] public long Value { get; set; }
    }

    #endregion CATEGORY

    #region EXTRA

    public class SpeedrunExtra
    {
        [JsonProperty("data")] public ExtraData Data { get; set; }
    }

    public class ExtraData
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonIgnore]
        [JsonProperty("released")]
        public int? Released { get; set; }

        [JsonProperty("links")] public List<Link> Links { get; set; }
    }

    public enum SpeedrunExtras
    {
        Platforms,
        Genres,
        Developers,
        Publishers
    }

    #endregion EXTRA
}