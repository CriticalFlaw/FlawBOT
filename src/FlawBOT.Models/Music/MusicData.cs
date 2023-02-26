using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using Newtonsoft.Json;

namespace FlawBOT.Models.Music
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
}