using Newtonsoft.Json;
using System;

namespace FlawBOT.Models
{
    public class TwitchData
    {
        [JsonProperty("stream")]
        public Stream stream { get; set; }
    }

    public class Channel
    {
        [JsonProperty("mature")]
        public bool mature { get; set; }

        [JsonProperty("partner")]
        public bool partner { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("broadcaster_language")]
        public string broadcaster_language { get; set; }

        [JsonProperty("broadcaster_software")]
        public string broadcaster_software { get; set; }

        [JsonProperty("display_name")]
        public string display_name { get; set; }

        [JsonProperty("game")]
        public string game { get; set; }

        [JsonProperty("language")]
        public string language { get; set; }

        [JsonProperty("_id")]
        public int _id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }

        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }

        [JsonProperty("delay")]
        public object delay { get; set; }

        [JsonProperty("logo")]
        public string logo { get; set; }

        [JsonProperty("banner")]
        public object banner { get; set; }

        [JsonProperty("video_banner")]
        public string video_banner { get; set; }

        [JsonProperty("background")]
        public object background { get; set; }

        [JsonProperty("profile_banner")]
        public string profile_banner { get; set; }

        [JsonProperty("profile_banner_background_color")]
        public string profile_banner_background_color { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("views")]
        public int views { get; set; }

        [JsonProperty("followers")]
        public int followers { get; set; }

        [JsonProperty("_links")]
        public Links _links { get; set; }
    }

    public class Stream
    {
        [JsonProperty("_id")]
        public long _id { get; set; }

        [JsonProperty("game")]
        public string game { get; set; }

        [JsonProperty("viewers")]
        public int viewers { get; set; }

        [JsonProperty("video_height")]
        public int video_height { get; set; }

        [JsonProperty("average_fps")]
        public double average_fps { get; set; }

        [JsonProperty("delay")]
        public int delay { get; set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }

        [JsonProperty("is_playlist")]
        public bool is_playlist { get; set; }

        [JsonProperty("stream_type")]
        public string stream_type { get; set; }

        [JsonProperty("preview")]
        public Preview preview { get; set; }

        [JsonProperty("channel")]
        public Channel channel { get; set; }

        [JsonProperty("_links")]
        public Links2 _links { get; set; }
    }

    public class Preview
    {
        [JsonProperty("small")]
        public string small { get; set; }

        [JsonProperty("medium")]
        public string medium { get; set; }

        [JsonProperty("large")]
        public string large { get; set; }

        [JsonProperty("template")]
        public string template { get; set; }
    }

    public class Links
    {
        [JsonProperty("self")]
        public string self { get; set; }

        [JsonProperty("follows")]
        public string follows { get; set; }

        [JsonProperty("commercial")]
        public string commercial { get; set; }

        [JsonProperty("stream_key")]
        public string stream_key { get; set; }

        [JsonProperty("chat")]
        public string chat { get; set; }

        [JsonProperty("features")]
        public string features { get; set; }

        [JsonProperty("subscriptions")]
        public string subscriptions { get; set; }

        [JsonProperty("editors")]
        public string editors { get; set; }

        [JsonProperty("teams")]
        public string teams { get; set; }

        [JsonProperty("videos")]
        public string videos { get; set; }
    }

    public class Links2
    {
        [JsonProperty("self")]
        public string self { get; set; }
    }

    public class Links3
    {
        [JsonProperty("self")]
        public string self { get; set; }

        [JsonProperty("channel")]
        public string channel { get; set; }
    }
}