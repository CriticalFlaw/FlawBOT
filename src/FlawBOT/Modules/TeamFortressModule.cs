using DSharpPlus.Entities;
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
        public async Task Tf2Creator(InteractionContext ctx, [Option("query", "Steam ID or custom URL name of the community creator.")] string query)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var steamId = await SteamService.GetSteamUserIdAsync(Program.Settings.Tokens.SteamToken, query).ConfigureAwait(false);
            var output = steamId is null ? null : await TeamFortressService.GetContentCreatorAsync(Program.Settings.Tokens.TeamworkToken, steamId.Value).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON).ConfigureAwait(false);
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
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await TeamFortressService.GetMapStatsAsync(Program.Settings.Tokens.TeamworkToken, query?.ToLowerInvariant()).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON).ConfigureAwait(false);
        }

        [SlashCommand("news", "Returns the latest news article from teamwork.tf")]
        public async Task Tf2News(InteractionContext ctx, [Option("query", "Page number from which to retrieve the news")][Minimum(0)] long query = 0)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await TeamFortressService.GetNewsArticlesAsync(Program.Settings.Tokens.TeamworkToken, (int)query).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON).ConfigureAwait(false);
        }

        [SlashCommand("search", "Returns a game server with given ip address.")]
        public async Task Tf2ServerSearch(InteractionContext ctx, [Option("query", "Game server IP address, like 164.132.233.16")] string ip)
        {
            if (string.IsNullOrWhiteSpace(ip) || !IPAddress.TryParse(ip.Trim(), out var address))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_INVALID_IP, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            // Send the connection link and the server banner in a single response; an interaction can only be answered once.
            var output = new DiscordEmbedBuilder()
                .WithDescription($"steam://connect/{address}:27015")
                .WithImageUrl(TeamFortressService.GetServerBanner(Program.Settings.Tokens.TeamworkToken, address.ToString()))
                .WithColor(new DiscordColor("#E7B53B"));
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        [SlashCommand("server", "Returns a list of servers for a given game-mode.")]
        public async Task Tf2Server(InteractionContext ctx, [Option("query", "Name of the game-mode, like payload.")] string query)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await TeamFortressService.GetServersByGameModeAsync(Program.Settings.Tokens.TeamworkToken, query?.Trim().Replace(' ', '-')).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON).ConfigureAwait(false);
        }

        [SlashCommand("server-list", "Returns a curated list of servers.")]
        public async Task Tf2ServerList(InteractionContext ctx)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await TeamFortressService.GetCustomServerListsAsync(Program.Settings.Tokens.TeamworkToken).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON, content: "Community-Curated Server Lists from teamwork.tf").ConfigureAwait(false);
        }
    }
}
