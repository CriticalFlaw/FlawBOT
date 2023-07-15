using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Net;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("tf2", "Slash command group for Team Fortress 2 commands.")]
    public class TeamFortressModule : ApplicationCommandModule
    {
        [SlashCommand("creator", "Returns a community creator profile from teamwork.tf")]
        public async Task Tf2Creator(InteractionContext ctx, [Option("query", "Name of the community creator to find.")] string query)
        {
            var steamId = SteamService.GetSteamUserId(query).Result;
            if (steamId is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            var output = await TeamFortressService.GetContentCreatorAsync(Program.Settings.Tokens.TeamworkToken, steamId.Data).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        [SlashCommand("item", "Returns an item from the latest TF2 item schema.")]
        public async Task Tf2Schema(InteractionContext ctx, [Option("query", "Item to find in the TF2 schema")] string query = "The Scattergun")
        {
            var output = TeamFortressService.GetSchemaItem(query);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        [SlashCommand("map", "Returns map information from teamwork.tf")]
        public async Task Tf2Map(InteractionContext ctx, [Option("query", "Normalized map name, like pl_upward")] string query)
        {
            var output = await TeamFortressService.GetMapStatsAsync(Program.Settings.Tokens.TeamworkToken, query.ToLowerInvariant()).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        [SlashCommand("news", "Returns the latest news article from teamwork.tf")]
        public async Task Tf2News(InteractionContext ctx, [Option("query", "Page number from which to retrieve the news")] double query = 0)
        {
            var output = await TeamFortressService.GetNewsArticlesAsync(Program.Settings.Tokens.TeamworkToken, (int)query).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        [SlashCommand("search", "Returns a game server with given ip address.")]
        public async Task Tf2ServerSearch(InteractionContext ctx, [Option("query", "Game server IP address, like 164.132.233.16")] string ip)
        {
            if (string.IsNullOrWhiteSpace(ip) || !IPAddress.TryParse(ip, out var address))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_INVALID_IP, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            await ctx.CreateResponseAsync(string.Format(Resources.URL_Steam_Connect, address)).ConfigureAwait(false);
            await ctx.CreateResponseAsync(TeamFortressService.GetServerBanner(Program.Settings.Tokens.TeamworkToken, address.ToString())).ConfigureAwait(false);
        }

        [SlashCommand("server", "Returns a list of servers for a given game-mode.")]
        public async Task Tf2Server(InteractionContext ctx, [Option("query", "Name of the game-mode, like payload.")] string query)
        {
            var output = await TeamFortressService.GetServersByGameModeAsync(Program.Settings.Tokens.TeamworkToken, query.Trim().Replace(' ', '-')).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        [SlashCommand("server-list", "Returns a curated list of servers.")]
        public async Task Tf2ServerList(InteractionContext ctx)
        {
            var output = await TeamFortressService.GetCustomServerListsAsync(Program.Settings.Tokens.TeamworkToken).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync("Community-Curated Server Lists from teamwork.tf", output).ConfigureAwait(false);
        }
    }
}