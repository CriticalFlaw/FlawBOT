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
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class TeamFortressModule : BaseCommandModule
    {
        #region COMMAND_NEWS

        [Command("news")]
        [Description("Get the latest news article from teamwork.tf")]
        public async Task TF2News(CommandContext ctx)
        {
            var data = await TeamFortressService.GetNewsOverviewAsync();
            if (data.Count <= 0)
                await BotServices.SendEmbedAsync(ctx, ":mag: No results found!", EmbedType.Warning);
            else
            {
                foreach (var value in data)
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(value.title)
                        .WithDescription("Provided by " + value.provider + " on " + value.created_at.date)
                        .WithFooter("Type next for the next news article")
                        .WithUrl(value.link)
                        .WithColor(DiscordColor.Orange);
                    await ctx.RespondAsync(embed: output.Build());

                    var interactivity = await ctx.Client.GetInteractivity()
                        .WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
                    if (interactivity == null) break;
                }
            }
        }

        #endregion COMMAND_NEWS

        #region COMMAND_SERVERS

        [Command("servers")]
        [Aliases("server")]
        [Description("Get a list of servers with given gamemode")]
        public async Task TF2Servers(CommandContext ctx, [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(ctx, query).Result) return;
            var data = await TeamFortressService.GetServersAsync(query.Trim().Replace(' ', '-'));
            if (data.Count <= 0)
                await BotServices.SendEmbedAsync(ctx, ":mag: No results found!", EmbedType.Warning);
            else
            {
                foreach (var value in data)
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(value.name)
                        .WithDescription("steam://connect/" + value.ip + ":" + value.port)
                        .AddField("Reachable", value.reachable ? "YES" : "NO", true)
                        //.AddField("Provider", value.provider ?? "Unknown", true)
                        .AddField("Secure", value.valve_secure ? "YES" : "NO", true)
                        .AddField("Current Map", value.map_name ?? "Unknown", true)
                        //.AddField("Next Map", value.map_name_next ?? "Unknown", true)
                        .AddField("Max Players", value.max_players.ToString() ?? "Unknown", true)
                        //.AddField("Password", value.has_password ? "YES" : "NO", true)
                        .AddField("Roll the Dice", value.has_rtd ? "YES" : "NO", true)
                        //.AddField("Random Crits", value.has_randomcrits.ToString() ?? "Unknown", true)
                        .AddField("Respawn Timer", value.has_norespawntime ? "YES" : "NO", true)
                        .AddField("All Talk", value.has_alltalk ? "YES" : "NO", true)
                        .WithThumbnailUrl("https://teamwork.tf" + value.map_name_thumbnail)
                        .WithFooter("Type next for the next server")
                        .WithColor(DiscordColor.Orange);
                    await ctx.RespondAsync(embed: output.Build());

                    var interactivity = await ctx.Client.GetInteractivity()
                        .WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
                    if (interactivity == null) break;
                }
            }
        }

        #endregion COMMAND_SERVERS

        #region COMMAND_MAP

        [Command("map")]
        [Description("Retrieve map statistics for a certain map from teamwork.tf")]
        public async Task TF2Map(CommandContext ctx, string query)
        {
            if (!BotServices.CheckUserInput(ctx, query).Result) return;
            var data = await TeamFortressService.GetMapStatsAsync(query);
            if (data == null)
                await BotServices.SendEmbedAsync(ctx, ":mag: No results found!", EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(data.normalized_map_name)
                    .AddField("Official", data.official_map ? "YES" : "NO")
                    .WithFooter("Statistics retrieved from teamwork.tf - refreshed every 5 minutes")
                    .WithImageUrl(data.thumbnail)
                    .WithUrl("https://wiki.teamfortress.com/wiki/" + data.normalized_map_name)
                    .WithColor(DiscordColor.Orange);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_MAP

        #region COMMAND_CONNECT

        [Command("connect")]
        [Aliases("server")]
        [Description("Format TF2 connection information into a clickable link")]
        public async Task SteamServerLink(CommandContext ctx, [RemainingText] string link)
        {
            var regex = new Regex(@"connect\s*(?'ip'\S+)\s*;\s*password\s*(?'pw'\S+);?", RegexOptions.Compiled).Match(link);
            if (regex.Success)
                await ctx.RespondAsync(string.Format($"steam://connect/{regex.Groups["ip"].Value}/{regex.Groups["pw"].Value}"));
            else
                await BotServices.SendEmbedAsync(ctx, ":warning: Invalid connection info, follow the format: **connect 123.345.56.789:00000; password hello**", EmbedType.Warning);
        }

        #endregion COMMAND_CONNECT

        #region UNUSED

        [Command("item"), Hidden]
        [Description("Retrieve a Team Fortress item from schema")]
        public async Task TF2Item(CommandContext ctx)
        {
            var data = TeamFortressService.GetSchemaItemAsync().Result;
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            var wikiLink = data.ItemName.Replace(' ', '_');
            var classes = data.UsedByClasses.Aggregate<string, string>(null, (current, userClass) => current + $" {userClass} ");
            var output = new DiscordEmbedBuilder()
                .WithTitle(data.ItemName)
                .WithThumbnailUrl(data.ImageUrl)
                .WithUrl("https://wiki.teamfortress.com/wiki/" + wikiLink)
                .WithColor(DiscordColor.Orange);
            if (data.ItemDescription != null)
                output.WithDescription(data.ItemDescription);
            if (data.ItemSlot != null)
                output.AddField("Item Slot:", textInfo.ToTitleCase(data.ItemSlot), true);
            if (classes != null)
                output.AddField("Used by:", textInfo.ToTitleCase(classes), true);
            if (data.ModelPlayer != null)
                await ctx.RespondAsync(embed: output.Build());
        }

        [Command("wiki"), Hidden]
        [Description("Get a page from the Team Fortress 2 wiki")]
        public async Task TF2Wiki(CommandContext ctx, [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(ctx, query).Result) return;
            var data = TeamFortressService.GetWikiPageAsync(query).Result.Query.Pages[0];
            if (data == null)
                await BotServices.SendEmbedAsync(ctx, ":warning: A search query was not provided!", EmbedType.Warning);
            else if (data.Missing || data == null)
                await BotServices.SendEmbedAsync(ctx, ":mag: TF2Wiki page not found!", EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                .WithTitle(data.Title)
                .WithUrl(data.FullUrl)
                .WithColor(DiscordColor.Orange); ;
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion UNUSED
    }
}