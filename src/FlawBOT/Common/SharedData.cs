using System;
using System.Reflection;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Properties;

namespace FlawBOT.Common
{
    public static class SharedData
    {
        public static string Name { get; } = "FlawBOT";
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        public static string GitHubLink { get; } = Resources.URL_BOT_GitHub;
        public static string InviteLink { get; } = Resources.URL_BOT_Invite;
        public static DiscordColor DefaultColor { get; } = new DiscordColor("#00FF7F");
        public static DateTime ProcessStarted { get; set; }
        public static TokenData Tokens { get; set; } = new TokenData();
        public static int ShardCount { get; } = 1;
    }

    public enum ResponseType
    {
        Default,
        Warning,
        Missing,
        Error
    }

    public enum UserStateChange
    {
        Ban,
        Deafen,
        Kick,
        Mute,
        Unban,
        Undeafen,
        Unmute
    }
}