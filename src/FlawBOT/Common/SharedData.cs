using DSharpPlus.Entities;
using FlawBOT.Models;
using Steam.Models.TF2;
using System;
using System.Collections.Generic;
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
        public static Dictionary<uint, string> SteamAppList { get; set; } = new Dictionary<uint, string>();
        public static Dictionary<uint, SchemaItem> TF2ItemSchema { get; set; } = new Dictionary<uint, SchemaItem>();
        public static List<string> PokemonList { get; set; } = new List<string>();
        public static TokenData Tokens { get; set; } = new TokenData();
    }
}