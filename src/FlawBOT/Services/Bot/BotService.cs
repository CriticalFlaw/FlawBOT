using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models;
using Newtonsoft.Json;
using SteamWebAPI2.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class BotServices
    {
        public static async Task SendEmbedAsync(CommandContext ctx, string message, EmbedType type = EmbedType.Default)
        {
            var prefix = "";
            DiscordColor color;
            switch (type)
            {
                case EmbedType.Good:
                    color = DiscordColor.Green;
                    break;

                case EmbedType.Warning:
                    prefix = ":warning: ";
                    color = DiscordColor.Yellow;
                    break;

                case EmbedType.Missing:
                    prefix = ":mag: ";
                    color = DiscordColor.Wheat;
                    break;

                case EmbedType.Error:
                    prefix = ":no_entry: ";
                    color = DiscordColor.Red;
                    break;

                default:
                    color = SharedData.DefaultColor;
                    break;
            }
            var output = new DiscordEmbedBuilder()
                .WithTitle(prefix + message)
                .WithColor(color);
            await ctx.RespondAsync(embed: output.Build());
        }

        public static bool CheckUserInput(string input)
        {
            return (string.IsNullOrWhiteSpace(input)) ? false : true;
        }

        public static async Task<bool> RemoveMessage(DiscordMessage message)
        {
            try
            {
                await message.DeleteAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<MemoryStream> CheckImageInput(CommandContext ctx, string input)
        {
            var stream = new MemoryStream();
            if (!Uri.TryCreate(input, UriKind.Absolute, out var uriResult) && (!input.EndsWith(".img") || !input.EndsWith(".png") || !input.EndsWith(".jpg")))
                await SendEmbedAsync(ctx, "An image URL ending with .img, .png or .jpg is required!", EmbedType.Warning).ConfigureAwait(false);
            else
            {
                using (var client = new WebClient())
                {
                    var results = client.DownloadData(input);
                    stream.Write(results, 0, results.Length);
                    stream.Position = 0;
                }
            }
            return stream;
        }

        public void UpdateTokenList()
        {
            var json = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)).ReadToEnd();
            SharedData.Tokens = JsonConvert.DeserializeObject<TokenData>(json);
        }

        public static Task UpdateSteamList()
        {
            try
            {
                var games = new SteamApps(SharedData.Tokens.SteamToken).GetAppListAsync().Result.Data;
                SharedData.SteamAppList.Clear();
                foreach (var game in games)
                    if (!string.IsNullOrWhiteSpace(game.Name))
                        SharedData.SteamAppList.Add(Convert.ToUInt32(game.AppId), game.Name);
            }
            catch
            {
                Console.WriteLine("Error loading Steam games list...");
            }

            return Task.CompletedTask;
        }

        public static Task UpdateTF2Schema()
        {
            try
            {
                var schema = new EconItems(SharedData.Tokens.SteamToken, EconItemsAppId.TeamFortress2).GetSchemaForTF2Async().Result.Data;
                SharedData.TF2ItemSchema.Clear();
                foreach (var item in schema.Items)
                    if (!string.IsNullOrWhiteSpace(item.Name))
                        SharedData.TF2ItemSchema.Add(Convert.ToUInt32(item.DefIndex), item);
            }
            catch
            {
                Console.WriteLine("Error loading TF2 item schema...");
            }

            return Task.CompletedTask;
        }
    }
}