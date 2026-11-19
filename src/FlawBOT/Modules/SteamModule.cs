using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("steam", "Slash command group for Steam commands.")]
    public class SteamModule : ApplicationCommandModule
    {
        [SlashCommand("connect", "Formats a server connection string into a link.")]
        public async Task SteamConnect(InteractionContext ctx, [Option("link", "Connection string (ex. IP:PORT; password PASS)")] string link)
        {
            var output = SteamService.FormatConnectLink(link);
            if (output is null)
                await BotServices.SendResponseAsync(ctx, Resources.ERR_INVALID_IP_GAME, ResponseType.Warning).ConfigureAwait(false);
            else
                await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        [SlashCommand("game", "Returns information on a Steam game.")]
        public async Task SteamGame(InteractionContext ctx, [Option("query", "Game to find on Steam.")] string query = "Team Fortress 2")
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await SteamService.GetSteamGameAsync(Program.Settings.Tokens.SteamToken, query).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON).ConfigureAwait(false);
        }

        [SlashCommand("user", "Returns information on a Steam user.")]
        public async Task SteamUser(InteractionContext ctx, [Option("query", "Steam ID, profile link or custom URL name of the user.")] string query)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await SteamService.GetSteamProfileAsync(Program.Settings.Tokens.SteamToken, query).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_COMMON).ConfigureAwait(false);
        }
    }
}
