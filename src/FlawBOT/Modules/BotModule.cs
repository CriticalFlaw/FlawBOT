using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("bot", "Slash command group for bot commands.")]
    public class BotModule : ApplicationCommandModule
    {
        [SlashCommand("info", "Returns basic information about FlawBOT.")]
        public async Task GetBotInfo(InteractionContext ctx)
        {
            var settings = Program.Settings;
            var output = new DiscordEmbedBuilder()
                .WithTitle(settings.Name)
                .WithDescription("Multipurpose Discord bot written in C# with DSharpPlus.")
                .AddField(":clock1: Uptime", BotServices.GetCurrentUptime(), true)
                .AddField(":link: Links", $"[Documentation]({settings.DocsLink}) **|** [GitHub]({settings.GitHubLink})", true)
                .WithFooter($"Version {settings.Version}")
                .WithUrl(settings.GitHubLink)
                .WithColor(settings.DefaultColor).Build();
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }

        [SlashCommand("leave", "Removes FlawBOT from the server.")]
        [SlashRequireGuild]
        [SlashRequireUserPermissions(Permissions.Administrator)]
        public async Task LeaveServer(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $"Thank you for using {Program.Settings.Name}. Goodbye!").ConfigureAwait(false);
            await ctx.Guild.LeaveAsync().ConfigureAwait(false);
        }

        [SlashCommand("ping", "Pings the active FlawBOT instance.")]
        public async Task GetBotPing(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $":ping_pong: Pong! Ping: {Formatter.Bold(ctx.Client.Ping.ToString())}ms").ConfigureAwait(false);
        }

        [SlashCommand("uptime", "Returns the uptime of the active FlawBOT instance.")]
        public async Task GetBotUptime(InteractionContext ctx)
        {
            await BotServices.SendResponseAsync(ctx, $":clock1: {BotServices.GetCurrentUptime()}").ConfigureAwait(false);
        }

        #region OWNER-ONLY

        [SlashRequireOwner]
        [SlashCommand("activity", "Changes FlawBOT's activity.")]
        public async Task SetBotActivity(InteractionContext ctx, [Option("activity", "Activity name.")] string activity)
        {
            await ctx.Client.UpdateStatusAsync(new DiscordActivity(activity)).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{Program.Settings.Name} activity has been changed to {Formatter.Bold(activity)}.").ConfigureAwait(false);
        }

        [SlashRequireOwner]
        [SlashCommand("avatar", "Changes FlawBOT's avatar.")]
        public async Task SetBotAvatar(InteractionContext ctx, [Option("url", "Image URL in JPG, PNG or GIF format.")] string url)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            await using var stream = await BotServices.CheckImageInput(url).ConfigureAwait(false);
            if (stream is null)
            {
                await BotServices.SendDeferredAsync(ctx, Resources.URL_INVALID_IMG, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await ctx.Client.UpdateCurrentUserAsync(avatar: stream).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, $"{Program.Settings.Name} avatar has been updated.").ConfigureAwait(false);
        }

        [SlashRequireOwner]
        [SlashRequireGuild]
        [SlashCommand("nickname", "Changes FlawBOT's server nickname.")]
        public async Task SetBotNickname(InteractionContext ctx, [Option("nickname", "New nickname for FlawBOT. Leave empty to reset.")] string nickname = null)
        {
            var newName = string.IsNullOrWhiteSpace(nickname) ? null : nickname.Trim();
            if (newName?.Length > 32)
            {
                await BotServices.SendResponseAsync(ctx, "Nicknames must be 32 characters or less.", ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await ctx.Guild.CurrentMember.ModifyAsync(x => x.Nickname = newName).ConfigureAwait(false);
            await ctx.CreateResponseAsync(newName is null
                ? $"{Program.Settings.Name}'s nickname has been reset."
                : $"{Program.Settings.Name}'s nickname has been changed to {Formatter.Bold(newName)}.").ConfigureAwait(false);
        }

        [SlashRequireOwner]
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

        #endregion
    }
}
