using DSharpPlus.Entities;
using FlawBOT.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FlawBOT.Common
{
    public class SharedData
    {
        public static string Name = Assembly.GetExecutingAssembly().GetName().Name.ToString();
        public static string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string CommandsList = "https://docs.google.com/spreadsheets/d/15c0Q7Cm07wBRNeSFwkagwDOe6zk9rVMvlM7H_Y7nGUs/edit?usp=sharing";
        public static string InviteLink = "https://discordapp.com/oauth2/authorize?client_id=339833029013012483&scope=bot&permissions=66186303";
        public static string GitHubLink = "https://github.com/criticalflaw/flawbot";
        public static DiscordColor DefaultColor = new DiscordColor("#00FF7F");
        public static DateTime ProcessStarted;
        public static Dictionary<uint, string> SteamAppList = new Dictionary<uint, string>();
        public static List<string> PokemonList = new List<string>();
        public static TokenData Tokens = new TokenData();
    }
}