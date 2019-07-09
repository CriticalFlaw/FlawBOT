using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Models;
using FlawBOT.Services;
using FlawBOT.Services.Games;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Games
{
    [Group("tf2")]
    [Description("Commands related to Team Fortress 2")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class TeamFortressModule : BaseCommandModule
    {
        #region COMMAND_SCHEMA

        [Command("item"), Hidden]
        [Description("Retrieve an item from the latest TF2 item schema")]
        public async Task TF2Item(CommandContext ctx,
            [Description("Item to find in the TF2 schema")] [RemainingText] string query = "The Scattergun")
        {
            var item = TeamFortressService.GetSchemaItemAsync(query);
            if (item == null)
                await BotServices.SendEmbedAsync(ctx, "Item not found in the schema!", EmbedType.Missing);
            else
            {
                var textInfo = new CultureInfo("en-US", false).TextInfo;
                var classes = item.UsedByClasses.Aggregate<string, string>(null, (current, userClass) => current + $" {userClass} ");
                var output = new DiscordEmbedBuilder()
                    .WithTitle(item.ItemName)
                    .WithThumbnailUrl(item.ImageUrl)
                    .WithUrl("https://wiki.teamfortress.com/wiki/" + item.ItemName.Replace(' ', '_'))
                    .WithColor(new DiscordColor("#E7B53B"));
                if (!string.IsNullOrWhiteSpace(item.ItemDescription))
                    output.WithDescription(item.ItemDescription);
                if (!string.IsNullOrWhiteSpace(item.ItemSlot))
                    output.AddField("Item Slot:", textInfo.ToTitleCase(item.ItemSlot), true);
                if (!string.IsNullOrWhiteSpace(classes))
                    output.AddField("Used by:", textInfo.ToTitleCase(classes), true);
                if (!string.IsNullOrWhiteSpace(item.ModelPlayer))
                    await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_SCHEMA

        #region COMMAND_CONNECT

        [Command("connect")]
        [Description("Format TF2 connection information into a clickable link")]
        public async Task SteamServerLink(CommandContext ctx,
            [Description("Connection string")] [RemainingText] string link)
        {
            var regex = new Regex(@"\s*(?'ip'\S+)\s*", RegexOptions.Compiled).Match(link);
            if (regex.Success)
                await ctx.RespondAsync(string.Format($"steam://connect/{regex.Groups["ip"].Value}/{regex.Groups["pw"].Value}"));
            else
                await BotServices.SendEmbedAsync(ctx, "Invalid connection info, follow the format: 123.345.56.789:000; password hello", EmbedType.Warning);
        }

        #endregion COMMAND_CONNECT

        #region COMMAND_MAP

        [Command("map")]
        [Aliases("maps")]
        [Description("Retrieve map information from teamwork.tf")]
        public async Task TF2Map(CommandContext ctx,
            [Description("Normalized map name, like pl_upward")] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = await TeamFortressService.GetMapStatsAsync(query.ToLowerInvariant());
            if (results.normalized_map_name == null)
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                Double.TryParse(results.alltime_avg_players, out var avg_players);
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.normalized_map_name)
                    .AddField("Official", results.official_map ? "YES" : "NO", true)
                    .AddField("Avg. Players", Math.Round(avg_players, 2).ToString() ?? "Unknown", true)
                    .AddField("Highest Player Count", results.highest_players.ToString() ?? "Unknown", true)
                    .AddField("Highest Server Count", results.highest_servers.ToString() ?? "Unknown", true)
                    .WithFooter("Statistics retrieved from teamwork.tf - refreshed every 5 minutes")
                    .WithImageUrl(results.thumbnail)
                    .WithUrl("https://wiki.teamfortress.com/wiki/" + results.normalized_map_name)
                    .WithColor(new DiscordColor("#E7B53B"));
                if (results.related_maps.Count > 0) output.AddField("Related Map", results.related_maps[0]);

                var related_maps = new StringBuilder();
                foreach (var map in results.related_maps.Take(5))
                    related_maps.Append(map + "\n");
                if (related_maps.Length > 0)
                    output.AddField("Related map(s)", related_maps.ToString(), true);

                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_MAP

        #region COMMAND_NEWS

        [Command("news")]
        [Description("Retrieve the latest news article from teamwork.tf")]
        public async Task TF2News(CommandContext ctx)
        {
            var results = await TeamFortressService.GetNewsOverviewAsync();
            if (results == null || results.Count == 0)
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithFooter("These are the latest news articles retrieved from teamwork.tf")
                    .WithColor(new DiscordColor("#E7B53B"));
                foreach (var result in results.Take(5))
                    output.AddField(result.title, result.link);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_NEWS

        #region COMMAND_SERVERS

        [Command("server")]
        [Aliases("servers")]
        [Description("Retrieve a list of servers with given gamemode")]
        public async Task TF2Servers(CommandContext ctx,
            [Description("Name of the gamemode, like payload")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            query = TeamFortressService.NormalizedGameMode(query);
            var results = await TeamFortressService.GetServersAsync(query.Trim().Replace(' ', '-'));
            if (results.Count <= 0)
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                foreach (var server in results.Where(n => n.map_name.Contains(query)))
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(server.name)
                        .WithDescription("steam://connect/" + server.ip + ":" + server.port)
                        .AddField("Secure", server.valve_secure ? "YES" : "NO", true)
                        //.AddField("Password", server.has_password ? "YES" : "NO", true)
                        .AddField("Max Players", (server.players.ToString() ?? "Unknown") + "/" + (server.max_players.ToString() ?? "Unknown"), true)
                        .AddField("Current Map", server.map_name ?? "Unknown", true)
                        .AddField("Next Map", server.map_name_next ?? "Unknown", true)
                        .AddField("Provider", server.provider ?? "Unknown", true)
                        .AddField("Roll the Dice", server.has_rtd ? "YES" : "NO", true)
                        //.AddField("Random Crits", server.has_randomcrits.ToString() ?? "Unknown", true)
                        .AddField("Respawn Timer", server.has_norespawntime ? "YES" : "NO", true)
                        .AddField("All Talk", server.has_alltalk ? "YES" : "NO", true)
                        .WithThumbnailUrl("https://teamwork.tf" + server.map_name_thumbnail)
                        .WithFooter("Type next in the next 10 seconds for the next server")
                        .WithColor(new DiscordColor("#E7B53B"));
                    var message = await ctx.RespondAsync(embed: output.Build());

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
                    if (interactivity.Result == null) break;
                    await BotServices.RemoveMessage(interactivity.Result);
                    await BotServices.RemoveMessage(message);
                }
            }
        }

        #endregion COMMAND_SERVERS

        #region BACKPACK.TF

        [Command("price_history"), Hidden]
        [Description("Retrieve price history for the specified item")]
        public async Task BackpackPriceHistory(CommandContext ctx,
            [RemainingText] string itemName)
        {
            var results = TeamFortressService.GetPriceHistory(itemName).Response;
            if (results.Success > 0)
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(itemName)
                    .WithColor(DiscordColor.Chartreuse);

                var histories = new StringBuilder();
                foreach (var history in results.History.Take(5))
                    histories.Append(history + "\n");
                if (histories.Length > 0)
                    output.AddField("History", histories.ToString(), true);

                await ctx.RespondAsync(embed: output.Build());
            }
        }

        [Command("item_prices"), Hidden]
        [Description("Retrieve item prices for the specified API key. A request may be sent once every 60 seconds")]
        public async Task BackpackItemPrices(CommandContext ctx)
        {
            var results = TeamFortressService.GetItemPrices().Response;
            if (results.Success > 0)
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                // Return results
            }
        }

        [Command("special_items"), Hidden]
        [Description("Retrieve special items for the specified API key")]
        public async Task BackpackSpecialItems(CommandContext ctx)
        {
            var results = TeamFortressService.GetSpecialItems().Response;
            if (results.Success > 0)
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                // Return results
            }
        }

        [Command("classifieds_all"), Hidden]
        [Description("Retrieve all currently open classifieds that are on backpack.tf")]
        public async Task BackpackClassifieds(CommandContext ctx)
        {
            var results = TeamFortressService.GetClassifieds();
            if (results == null)
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                // Return results
            }
        }

        [Command("classifieds_my"), Hidden]
        [Description("Retrieve the currently opened user's classifieds from backpack.tf")]
        public async Task BackpackOwnClassifieds(CommandContext ctx)
        {
            var results = TeamFortressService.GetOwnClassifieds();
            if (results == null)
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                // Return results
            }
        }

        [Command("inventory_user"), Hidden]
        [Description("Retrieve the currently opened user's classifieds from backpack.tf")]
        public async Task BackpackUserInventory(CommandContext ctx, string steamID)
        {
            var results = TeamFortressService.GetUserInventory(steamID);
            if (results.Success > 0)
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                // Return results
            }
        }

        [Command("inventory_my"), Hidden]
        [Description("Retrieve the currently opened user's classifieds from backpack.tf")]
        public async Task BackpackOwnInventory(CommandContext ctx)
        {
            var results = TeamFortressService.GetOwnInventory();
            if (results.Success > 0)
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                // Return results
            }
        }

        [Command("inventory_item"), Hidden]
        [Description("Retrieve an item in the user's inventory and returns its Asset and Description models")]
        public async Task BackpackItemFromInventory(CommandContext ctx, string itemName)
        {
            var results = TeamFortressService.GetItemFromInventory(itemName);
            if (results == null)
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                // Return results
            }
        }

        #endregion BACKPACK.TF
    }
}