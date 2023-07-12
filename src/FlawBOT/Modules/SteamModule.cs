using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("steam", "Slash command group for Steam commands.")]
    public class SteamModule : ApplicationCommandModule
    {
        [SlashCommand("connect", "Formats a server connection string into a link.")]
        public async Task SteamConnect(InteractionContext ctx, [Option("link", "Connection string (ex. IP:PORT)")] string link)
        {
            var regex = new Regex(@"\s*(?'ip'\S+)\s*", RegexOptions.Compiled).Match(link);
            if (regex.Success)
                await ctx.CreateResponseAsync(string.Format($"steam://connect/{regex.Groups["ip"].Value}/{regex.Groups["pw"].Value}")).ConfigureAwait(false);
            else
                await BotServices.SendResponseAsync(ctx, Resources.ERR_INVALID_IP_GAME, ResponseType.Warning).ConfigureAwait(false);
        }

        [SlashCommand("game", "Returns information on a Steam game.")]
        public async Task SteamGame(InteractionContext ctx, [Option("query", "Game to find on Steam.")] string query = "Team Fortress 2")
        {
            var output = await SteamService.GetSteamGame(query).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        [SlashCommand("user", "Returns information on a Steam user.")]
        public async Task SteamUser(InteractionContext ctx, [Option("query", "User to find on Steam.")] string query)
        {
            var output = SteamService.GetSteamProfileAsync(Program.Settings.Tokens.SteamToken, query).Result;
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}