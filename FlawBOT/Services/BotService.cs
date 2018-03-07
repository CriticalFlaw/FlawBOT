using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        }
    }

    public class HelperService : IHelpFormatter
    {
        public HelperService()
        {
            MessageBuilder = new StringBuilder();
        }

        private StringBuilder MessageBuilder { get; }

        public IHelpFormatter WithCommandName(string name)
        {
            MessageBuilder.Append("**Command**: ").AppendLine(name);
            return this;
        }

        public IHelpFormatter WithAliases(IEnumerable<string> aliases)
        {
            MessageBuilder.Append("**Aliases**: ").AppendLine(string.Join(", ", aliases));
            return this;
        }

        public IHelpFormatter WithDescription(string description)
        {
            MessageBuilder.Append("**Description**: ").AppendLine(description);
            return this;
        }

        public IHelpFormatter WithArguments(IEnumerable<CommandArgument> arguments)
        {
            MessageBuilder.Append("**Arguments**: ").AppendLine(string.Join(", ", arguments.Select(xarg => $"{xarg.Name} ({xarg.Type.ToUserFriendlyName()})")));
            return this;
        }

        public IHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            MessageBuilder.Append("**Commands**: ").AppendLine(string.Join(", ", subcommands.Select(xc => xc.Name)));
            return this;
        }

        public IHelpFormatter WithGroupExecutable()
        {
            MessageBuilder.AppendLine("This group is a standalone command.").AppendLine();
            return this;
        }

        public CommandHelpMessage Build()
        {
            return new CommandHelpMessage(MessageBuilder.ToString().Replace("\r\n", "\n"));
        }
    }

    public class MathService : IArgumentConverter<MathOperations>
    {
        public bool TryConvert(string value, CommandContext ctx, out MathOperations result)
        {
            switch (value)
            {
                case "+":
                    result = MathOperations.Add;
                    return true;

                case "-":
                    result = MathOperations.Subtract;
                    return true;

                case "*":
                    result = MathOperations.Multiply;
                    return true;

                case "/":
                    result = MathOperations.Divide;
                    return true;

                case "%":
                    result = MathOperations.Modulo;
                    return true;

                default:
                    result = MathOperations.Add;
                    return false;
            }
        }
    }

    public enum MathOperations
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo
    }
}