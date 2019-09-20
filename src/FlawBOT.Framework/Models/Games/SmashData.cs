using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    public class SmashCharacter
    {
        [JsonProperty("InstanceId")]
        public string InstanceId { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("OwnerId")]
        public int OwnerId { get; set; }

        [JsonProperty("FullUrl")]
        public string FullUrl { get; set; }

        [JsonProperty("MainImageUrl")]
        public string MainImageUrl { get; set; }

        [JsonProperty("ThumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("ColorTheme")]
        public string ColorTheme { get; set; }

        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }

        [JsonProperty("Game")]
        public string Game { get; set; }
    }

    public class SmashCharacterAttributes
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Values")]
        public List<Attribute> Attributes { get; set; }
    }

    public class Attribute
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}