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

        [JsonProperty("Related")]
        public Related Related { get; set; }
    }

    public class Related
    {
        [JsonProperty("Ultimate")]
        public Ultimate Ultimate { get; set; }
    }

    public class Ultimate
    {
        [JsonProperty("Self")]
        public string Self { get; set; }

        [JsonProperty("Moves")]
        public string Moves { get; set; }

        [JsonProperty("Movements")]
        public string Movements { get; set; }

        [JsonProperty("Attributes")]
        public string Attributes { get; set; }
    }
}