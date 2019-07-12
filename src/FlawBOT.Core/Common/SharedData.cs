using DSharpPlus.Entities;
using System;
using System.Reflection;

namespace FlawBOT.Common
{
    public class SharedData
    {
        public static string Name { get; } = Assembly.GetExecutingAssembly().GetName().Name;
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string GitHubLink { get; set; } = "https://github.com/CriticalFlaw/FlawBOT/";
        public static string InviteLink { get; } = "https://discordapp.com/oauth2/authorize?client_id=339833029013012483&scope=bot&permissions=66186303";
        public static DiscordColor DefaultColor { get; set; } = new DiscordColor("#00FF7F");
        public static DateTime ProcessStarted { get; set; }
    }
}