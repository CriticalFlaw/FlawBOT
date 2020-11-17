using System;
using System.Reflection;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;

namespace FlawBOT.Common
{
    public static class SharedData
    {
        public static string Name { get; } = "FlawBOT";
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        public static string GitHubLink { get; } = Resources.BOT_GITHUB;
        public static string InviteLink { get; } = Resources.BOT_INVITE;
        public static DiscordColor DefaultColor { get; } = new DiscordColor("#00FF7F");
        public static DateTime ProcessStarted { get; set; }
    }
}