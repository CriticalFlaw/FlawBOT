using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using Newtonsoft.Json;

namespace FlawBOT.Models
{
    public struct MusicData
    {
        [JsonIgnore] public LavalinkTrack Track { get; }

        [JsonIgnore] public DiscordMember Requester { get; }

        public MusicData(LavalinkTrack track, DiscordMember requester)
        {
            Track = track;
            Requester = requester;
        }
    }

    public struct YouTubeData
    {
        public string Title { get; }
        public string Author { get; }
        public string Id { get; }

        public YouTubeData(string title, string author, string id)
        {
            Title = title;
            Author = author;
            Id = id;
        }
    }

    internal struct YouTubeResponse
    {
        [JsonProperty("id")] public ResponseId Id { get; private set; }

        [JsonProperty("snippet")] public ResponseSnippet Snippet { get; private set; }

        public struct ResponseId
        {
            [JsonProperty("videoId")] public string VideoId { get; private set; }
        }

        public struct ResponseSnippet
        {
            [JsonProperty("title")] public string Title { get; private set; }

            [JsonProperty("channelTitle")] public string Author { get; private set; }
        }
    }
}