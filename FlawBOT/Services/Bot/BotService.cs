using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using FlawBOT.Models;
using Newtonsoft.Json;
using SteamWebAPI2.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    internal class GlobalVariables : Random
    {
        public static string Name = "FlawBOT";
        public static string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static DateTime ProcessStarted;
        public static Dictionary<uint, string> SteamAppList = new Dictionary<uint, string>();
        public static Dictionary<uint, string> TFItemSchema = new Dictionary<uint, string>();
        public static BotConfig config = new BotConfig();
        private static int _seed;
        private static readonly ThreadLocal<Random> ThreadLocal = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));
        public static Random Instance => ThreadLocal.Value;

        static GlobalVariables()
        {
            _seed = Environment.TickCount;
        }
    }

    public class BotServices
    {
        public void LoadBotConfig()
        {
            using (var stream = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)))
            {
                var json = stream.ReadToEnd();
                GlobalVariables.config = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        public static async Task SendEmbedAsync(CommandContext ctx, string message, EmbedType type = EmbedType.Default)
        {
            var color = DiscordColor.DarkButNotBlack;
            switch (type)
            {
                case EmbedType.Good:
                    color = DiscordColor.SpringGreen;
                    break;

                case EmbedType.Warning:
                    color = DiscordColor.Yellow;
                    break;

                case EmbedType.Error:
                    color = DiscordColor.Red;
                    break;
            }
            var output = new DiscordEmbedBuilder()
                .WithTitle(message)
                .WithColor(color);
            await ctx.RespondAsync(embed: output.Build());
        }

        public static async Task<bool> CheckUserInput(CommandContext ctx, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                await SendEmbedAsync(ctx, ":mag: Please provide a search query!", EmbedType.Warning);
                return false;
            }
            return true;
        }

        public static async Task<MemoryStream> CheckImageInput(CommandContext ctx, string input)
        {
            var stream = new MemoryStream();
            if (!Uri.TryCreate(input, UriKind.Absolute, out var uriResult) && (!input.EndsWith(".img") || !input.EndsWith(".png") || !input.EndsWith(".jpg")))
                await SendEmbedAsync(ctx, ":warning: An image URL ending with .img, .png or .jpg is required!", EmbedType.Warning);
            else
            {
                using (var client = new WebClient())
                {
                    var data = client.DownloadData(input);
                    stream.Write(data, 0, data.Length);
                    stream.Position = 0;
                }
            }
            return stream;
        }

        public static Task UpdateSteamAsync()
        {
            try
            {
                var token = GlobalVariables.config.SteamToken;
                var games = new SteamApps(token).GetAppListAsync().Result.Data;
                GlobalVariables.SteamAppList.Clear();
                foreach (var game in games)
                    if (!string.IsNullOrWhiteSpace(game.Name))
                        GlobalVariables.SteamAppList.Add(Convert.ToUInt32(game.AppId), game.Name);

                //Console.WriteLine("Updating TF2 Item Schema...");
                //var schema = new EconItems(token, EconItemsAppId.TeamFortress2);
                //var items = schema.GetSchemaForTF2Async();
                //GlobalVariables.TFItemSchema.Clear();
                //foreach (var item in items.Result.Data.Items)
                //    if (!string.IsNullOrWhiteSpace(item.ItemName))
                //        GlobalVariables.TFItemSchema.Add(Convert.ToUInt32(item.DefIndex), item.ItemName);
            }
            catch
            {
                Console.WriteLine("Error updating Steam libraries...");
            }

            return Task.CompletedTask;
        }
    }

    #region HELPER_SERVICE

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
            var embed = new DiscordEmbedBuilder(hmsg.Embed);
            embed.Color = new DiscordColor(0xD091B2);
            return new CommandHelpMessage(embed: embed);
        }
    }

    #endregion HELPER_SERVICE
}