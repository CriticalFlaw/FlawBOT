using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("bot", "Slash command group for bot commands.")]
    public class BotModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns basic information about FlawBOT.
        /// </summary>
        [SlashCommand("info", "Returns basic information about FlawBOT.")]
        public async Task GetBotInfo(InteractionContext ctx)
        {
            var settings = Program.Settings;
            var output = new DiscordEmbedBuilder()
                .WithTitle(settings.Name)
                .WithDescription($"Multipurpose Discord bot written in C# with DSharpPlus.")
                .AddField(":clock1: Uptime", BotServices.GetCurrentUptime(), true)
                .AddField(":link: Links", $"[Commands]({settings.DocsLink}) **|** [Commands]({settings.DocsLink}) **|** [GitHub]({settings.GitHubLink})", true)
                .WithFooter($"Version {settings.Version}")
                .WithUrl(settings.GitHubLink)
                .WithColor(settings.DefaultColor).Build();
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        /// <summary>
        /// Ping the active FlawBOT instance.
        /// </summary>
        [SlashCommand("ping", "Ping the active FlawBOT instance.")]
        public async Task GetBotPing(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $":ping_pong: Pong! Ping: {Formatter.Bold(ctx.Client.Ping.ToString())}ms").ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the uptime of the active FlawBOT instance.
        /// </summary>
        [SlashCommand("uptime", "Returns the uptime of the active FlawBOT instance.")]
        public async Task GetBotUptime(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $":clock1: {BotServices.GetCurrentUptime()}").ConfigureAwait(false);
        }

        /// <summary>
        /// Changes FlawBOT's activity.
        /// </summary>
        [RequireOwner]
        [SlashCommand("activity", "Changes FlawBOT's activity.")]
        public async Task SetBotActivity(InteractionContext ctx, [Option("activity", "Activity name.")] string activity)
        {
            await ctx.Client.UpdateStatusAsync(new DiscordActivity(activity)).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{Program.Settings.Name} activity has been changed to {Formatter.Bold(activity)}.").ConfigureAwait(false);
        }

        /// <summary>
        /// Changes FlawBOT's avatar.
        /// </summary>
        [RequireOwner]
        [SlashCommand("avatar", "Changes FlawBOT's avatar.")]
        public async Task SetBotAvatar(InteractionContext ctx, [Option("url", "Image URL in JPG, PNG or IMG format.")] string url)
        {
            var stream = BotServices.CheckImageInput(ctx, url).Result;
            if (stream is null)
            {
                await ctx.CreateResponseAsync("Invalid image format. Please try again.").ConfigureAwait(false);
                return;
            }
            await ctx.Client.UpdateCurrentUserAsync(avatar: stream).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{Program.Settings.Name} avatar has been updated.").ConfigureAwait(false);
        }

        /// <summary>
        /// Changes FlawBOT's status.
        /// </summary>
        [RequireOwner]
        [SlashCommand("status", "Changes FlawBOT's status.")]
        public async Task SetBotStatus(InteractionContext ctx, [Choice("Online", "Online")][Choice("Idle", "Idle")][Choice("Invisible", "Invisible")][Choice("Do Not Disturb", "Do Not Disturb")][Option("status", "New FlawBOT status.")] string status)
        {
            var state = status switch
            {
                "Idle" => UserStatus.Idle,
                "Invisible" => UserStatus.Invisible,
                "Do Not Disturb" => UserStatus.DoNotDisturb,
                _ => UserStatus.Online
            };
            await ctx.Client.UpdateStatusAsync(userStatus: state).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{Program.Settings.Name} status has been changed to {status}.").ConfigureAwait(false);
        }

        /// <summary>
        /// Changes FlawBOT's nickname.
        /// </summary>
        [RequireOwner]
        [SlashCommand("nickname", "Changes FlawBOT's nickname.")]
        public async Task SetBotUsername(InteractionContext ctx, [Option("nickname", "New nickname for FlawBOT.")] string nickname)
        {
            var oldName = ctx.Client.CurrentUser.Username;
            var newName = string.IsNullOrWhiteSpace(nickname) ? Program.Settings.Name : nickname;
            await ctx.Client.UpdateCurrentUserAsync(username: newName).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{oldName}'s username has been changed to {newName}.").ConfigureAwait(false);
        }

        /// <summary>
        /// Makes FlawBOT leave the current server.
        /// </summary>
        [SlashCommand("leave", "Makes FlawBOT leave the current server.")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task LeaveServer(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $"Thank you for using {Program.Settings.Name}. Goodbye!").ConfigureAwait(false);
            await ctx.Guild.LeaveAsync().ConfigureAwait(false);
        }
    }
}