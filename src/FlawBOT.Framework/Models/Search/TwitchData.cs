using System;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    public class TwitchData
    {
        [JsonProperty("stream")]
        public Stream Stream { get; set; }
    }

    public class Stream
    {
        [JsonProperty("game")]
        public string Game { get; set; }

        [JsonProperty("viewers")]
        public int Viewers { get; set; }

        [JsonProperty("average_fps")]
        public double AverageFPS { get; set; }

        [JsonProperty("delay")]
        public int Delay { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("stream_type")]
        public string StreamType { get; set; }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }
    }

    public class Channel
    {
        [JsonProperty("mature")]
        public bool IsMature { get; set; }

        [JsonProperty("partner")]
        public bool IsPartner { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("broadcaster_language")]
        public string BroadcasterLanguage { get; set; }

        [JsonProperty("broadcaster_software")]
        public string Software { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("game")]
        public string Game { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("_id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("delay")]
        public object Delay { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("banner")]
        public object Banner { get; set; }

        [JsonProperty("video_banner")]
        public string VideoBanner { get; set; }

        [JsonProperty("background")]
        public object Background { get; set; }

        [JsonProperty("profile_banner")]
        public string ProfileBanner { get; set; }

        [JsonProperty("profile_banner_background_color")]
        public string ProfileBannerColor { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("views")]
        public int Views { get; set; }

        [JsonProperty("followers")]
        public int Followers { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }

    public class Links
    {
        [JsonProperty("self")]
        public string Self { get; set; }

        [JsonProperty("follows")]
        public string Follows { get; set; }

        [JsonProperty("commercial")]
        public string Commercial { get; set; }

        [JsonProperty("stream_key")]
        public string StreamKey { get; set; }

        [JsonProperty("chat")]
        public string Chat { get; set; }

        [JsonProperty("features")]
        public string Features { get; set; }

        [JsonProperty("subscriptions")]
        public string Subscriptions { get; set; }

        [JsonProperty("editors")]
        public string Editors { get; set; }

        [JsonProperty("teams")]
        public string Teams { get; set; }

        [JsonProperty("videos")]
        public string Videos { get; set; }
    }
}