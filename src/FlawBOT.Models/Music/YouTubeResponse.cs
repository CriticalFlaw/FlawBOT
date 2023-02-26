using Newtonsoft.Json;

namespace FlawBOT.Models.Music
{
    public struct YouTubeResponse
    {
        [JsonProperty("id")]
        public ResponseId Id { get; private set; }

        [JsonProperty("snippet")]
        public ResponseSnippet Snippet { get; private set; }

        public struct ResponseId
        {
            [JsonProperty("videoId")]
            public string VideoId { get; private set; }
        }

        public struct ResponseSnippet
        {
            [JsonProperty("title")]
            public string Title { get; private set; }

            [JsonProperty("channelTitle")]
            public string Author { get; private set; }
        }
    }
}