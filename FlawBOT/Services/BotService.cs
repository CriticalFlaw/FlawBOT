using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace FlawBOT.Services
{
    internal class GlobalVariables : Random
    {
        public static string Name = "FlawBOT";
        public static string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static DateTime ProcessStarted;
        public static Dictionary<uint, string> SteamAppList = new Dictionary<uint, string>();
        private static int _seed;
        private static readonly ThreadLocal<Random> ThreadLocal = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));
        public static Random Instance => ThreadLocal.Value;

        //public static Dictionary<int, string> ItemSchema = new Dictionary<int, string>();
        static GlobalVariables()
        {
            _seed = Environment.TickCount;
        }
    }

    public class APITokenService
    {
        public string GetAPIToken(string query)
        {
            string json;
            using (var stream = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)))
                json = stream.ReadToEnd();

            switch (query.ToUpperInvariant())
            {
                case "DISCORD":
                    return JsonConvert.DeserializeObject<APITokenList>(json).Token;

                case "PREFIX":
                    return JsonConvert.DeserializeObject<APITokenList>(json).CommandPrefix;

                case "GOOGLE":
                    return JsonConvert.DeserializeObject<APITokenList>(json).GoogleToken;

                case "STEAM":
                    return JsonConvert.DeserializeObject<APITokenList>(json).SteamToken;

                case "IMGUR":
                    return JsonConvert.DeserializeObject<APITokenList>(json).ImgurToken;

                case "OMDB":
                    return JsonConvert.DeserializeObject<APITokenList>(json).OMDBToken;

                case "TWITCH":
                    return JsonConvert.DeserializeObject<APITokenList>(json).TwitchToken;

                case "DATABASE":
                    return JsonConvert.DeserializeObject<APITokenList>(json).ConnectionString;

                default:
                    return null;
            }
        }

        public struct APITokenList
        {
            [JsonProperty("token")] public string Token { get; private set; }

            [JsonProperty("prefix")] public string CommandPrefix { get; private set; }

            [JsonProperty("google")] public string GoogleToken { get; private set; }

            [JsonProperty("steam")] public string SteamToken { get; private set; }

            [JsonProperty("imgur")] public string ImgurToken { get; private set; }

            [JsonProperty("omdb")] public string OMDBToken { get; private set; }

            [JsonProperty("twitch")] public string TwitchToken { get; private set; }

            [JsonProperty("database")] public string ConnectionString { get; private set; }
        }
    }

    public sealed class HelperService : BaseHelpFormatter
    {
        private readonly DefaultHelpFormatter helper;

        public HelperService(CommandsNextExtension cnext) : base(cnext)
        {
            helper = new DefaultHelpFormatter(cnext);
        }

        public override BaseHelpFormatter WithCommand(Command command)
        {
            return helper.WithCommand(command);
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            return helper.WithSubcommands(subcommands);
        }

        public override CommandHelpMessage Build()
        {
            var hmsg = helper.Build();
            var embed = new DiscordEmbedBuilder(hmsg.Embed)
            {
                Color = new DiscordColor(0xD091B2)
            };
            return new CommandHelpMessage(embed: embed);
        }
    }
}