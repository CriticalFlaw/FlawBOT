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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Games
{
    [Group("tf2")]
    [Description("Commands related to Team Fortress 2")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class TeamFortressModule : BaseCommandModule
    {
        #region COMMAND_CONNECT

        [Command("connect")]
        [Description("Format TF2 connection information into a clickable link")]
        public async Task SteamServerLink(CommandContext ctx,
            [Description("Connection string")] [RemainingText] string link)
        {
            var regex = new Regex(@"connect\s*(?'ip'\S+)\s*;\s*password\s*(?'pw'\S+);?", RegexOptions.Compiled).Match(link);
            if (regex.Success)
                await ctx.RespondAsync(string.Format($"steam://connect/{regex.Groups["ip"].Value}/{regex.Groups["pw"].Value}"));
            else
                await BotServices.SendEmbedAsync(ctx, ":warning: Invalid connection info, follow the format: 123.345.56.789:000; password hello", EmbedType.Warning);
        }

        #endregion COMMAND_CONNECT

        #region COMMAND_MAP

        [Command("map")]
        [Aliases("maps")]
        [Description("Retrieve map information from teamwork.tf")]
        public async Task TF2Map(CommandContext ctx,
            [Description("Normalized map name, like pl_upward")] string query)
        {
            if (!BotServices.CheckUserInput(ctx, query).Result) return;
            var results = await TeamFortressService.GetMapStatsAsync(query);
            if (results.normalized_map_name == null)
                await BotServices.SendEmbedAsync(ctx, ":mag: No results found!", EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.normalized_map_name)
                    .AddField("Official", results.official_map ? "YES" : "NO")
                    .AddField("Avg. Players", results.alltime_avg_players)
                    .AddField("First Seen", results.first_seen.ToString())
                    .AddField("Highest Player Count", results.highest_players.ToString())
                    .AddField("Highest Server Count", results.highest_servers.ToString())
                    .WithFooter("Statistics retrieved from teamwork.tf - refreshed every 5 minutes")
                    .WithImageUrl(results.thumbnail)
                    .WithUrl("https://wiki.teamfortress.com/wiki/" + results.normalized_map_name)
                    .WithColor(new DiscordColor("#E7B53B"));
                if (results.related_maps.Count > 0) output.AddField("Related Map", results.related_maps[0]);
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
                await BotServices.SendEmbedAsync(ctx, ":mag: No results found!", EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder { Color = new DiscordColor("#E7B53B") };
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
            if (!BotServices.CheckUserInput(ctx, query).Result) return;
            var results = await TeamFortressService.GetServersAsync(query.Trim().Replace(' ', '-'));
            if (results.Count <= 0)
                await BotServices.SendEmbedAsync(ctx, ":mag: No results found!", EmbedType.Warning);
            else
            {
                foreach (var server in results)
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(server.name)
                        .WithDescription("steam://connect/" + server.ip + ":" + server.port)
                        //.AddField("Reachable", server.reachable ? "YES" : "NO", true)
                        //.AddField("Provider", server.provider ?? "Unknown", true)
                        .AddField("Secure", server.valve_secure ? "YES" : "NO", true)
                        .AddField("Current Map", server.map_name ?? "Unknown", true)
                        //.AddField("Next Map", server.map_name_next ?? "Unknown", true)
                        .AddField("Max Players", server.max_players.ToString() ?? "Unknown", true)
                        //.AddField("Password", server.has_password ? "YES" : "NO", true)
                        .AddField("Roll the Dice", server.has_rtd ? "YES" : "NO", true)
                        //.AddField("Random Crits", server.has_randomcrits.ToString() ?? "Unknown", true)
                        .AddField("Respawn Timer", server.has_norespawntime ? "YES" : "NO", true)
                        .AddField("All Talk", server.has_alltalk ? "YES" : "NO", true)
                        .WithThumbnailUrl("https://teamwork.tf" + server.map_name_thumbnail)
                        .WithFooter("Type next for the next server")
                        .WithColor(new DiscordColor("#E7B53B"));
                    var message = await ctx.RespondAsync(embed: output.Build());

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
                    if (interactivity == null) break;
                    await interactivity.Message.DeleteAsync();
                    await message.DeleteAsync();
                }
            }
        }

        #endregion COMMAND_SERVERS

        #region UNUSED

        [Command("item"), Hidden]
        [Description("Retrieve an item from the latest TF2 item schema")]
        public async Task TF2Item(CommandContext ctx)
        {
            var results = TeamFortressService.GetSchemaItemAsync().Result;
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            var wikiLink = results.ItemName.Replace(' ', '_');
            var classes = results.UsedByClasses.Aggregate<string, string>(null, (current, userClass) => current + $" {userClass} ");
            var output = new DiscordEmbedBuilder()
                .WithTitle(results.ItemName)
                .WithThumbnailUrl(results.ImageUrl)
                .WithUrl("https://wiki.teamfortress.com/wiki/" + wikiLink)
                .WithColor(DiscordColor.Orange);
            if (results.ItemDescription != null)
                output.WithDescription(results.ItemDescription);
            if (results.ItemSlot != null)
                output.AddField("Item Slot:", textInfo.ToTitleCase(results.ItemSlot), true);
            if (classes != null)
                output.AddField("Used by:", textInfo.ToTitleCase(classes), true);
            if (results.ModelPlayer != null)
                await ctx.RespondAsync(embed: output.Build());
        }

        [Command("wiki"), Hidden]
        [Description("Retrieve a page from the Team Fortress 2 wiki")]
        public async Task TF2Wiki(CommandContext ctx,
            [Description("Search query to take to the TF2 wiki")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(ctx, query).Result) return;
            var results = TeamFortressService.GetWikiPageAsync(query).Result.Query.Pages[0];
            if (results.Missing || results == null)
                await BotServices.SendEmbedAsync(ctx, ":mag: TF2Wiki page not found!", EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                .WithTitle(results.Title)
                .WithUrl(results.FullUrl)
                .WithColor(DiscordColor.Orange); ;
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion UNUSED
    }
}