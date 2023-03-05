using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [RequireOwner]
    [Hidden]
    [SlashCommandGroup("bot", "Slash command group for modal bot commands.")]
    public class AdminModule : ApplicationCommandModule
    {
        [SlashCommand("bot_activity", "Set FlawBOT's activity.")]
        public async Task SetBotActivity(InteractionContext ctx, [Option("activity", "Name of the activity.")] string activity)
        {
            if (string.IsNullOrWhiteSpace(activity))
            {
                await ctx.Client.UpdateStatusAsync().ConfigureAwait(false);
                return;
            }

            var game = new DiscordActivity(activity);
            await ctx.Client.UpdateStatusAsync(game).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{Program.Settings.Name} activity has been changed to Playing {game.Name}").ConfigureAwait(false);
        }

        [SlashCommand("bot_avatar", "Set FlawBOT's avatar.")]
        public async Task SetBotAvatar(InteractionContext ctx, [Option("query", "Image URL. Must be in JPG, PNG or IMG format.")] string query)
        {
            var stream = BotServices.CheckImageInput(ctx, query).Result;
            if (stream.Length <= 0) return;
            await ctx.Client.UpdateCurrentUserAsync(avatar: stream).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{Program.Settings.Name} avatar has been updated!").ConfigureAwait(false);
        }

        [SlashCommand("bot_status", "Set FlawBOT's status.")]
        public async Task SetBotStatus(InteractionContext ctx, [Option("status", "Activity Status. Online, Idle, DND or Offline.")] string status = "ONLINE")
        {
            switch (status.Trim().ToUpperInvariant())
            {
                case "OFF":
                case "OFFLINE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Offline).ConfigureAwait(false);
                    await ctx.CreateResponseAsync($"{Program.Settings.Name} status has been changed to Offline").ConfigureAwait(false);
                    break;

                case "INVISIBLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Invisible).ConfigureAwait(false);
                    await ctx.CreateResponseAsync($"{Program.Settings.Name} status has been changed to Invisible").ConfigureAwait(false);
                    break;

                case "IDLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Idle).ConfigureAwait(false);
                    await ctx.CreateResponseAsync($"{Program.Settings.Name} status has been changed to Idle").ConfigureAwait(false);
                    break;

                case "DND":
                case "DO NOT DISTURB":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.DoNotDisturb).ConfigureAwait(false);
                    await ctx.CreateResponseAsync($"{Program.Settings.Name} status has been changed to Do Not Disturb").ConfigureAwait(false);
                    break;

                default:
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Online).ConfigureAwait(false);
                    await ctx.CreateResponseAsync($"{Program.Settings.Name} status has been changed to Online").ConfigureAwait(false);
                    break;
            }
        }

        [SlashCommand("bot_username", "Set FlawBOT's username.")]
        public async Task SetBotUsername(InteractionContext ctx, [Option("name", "New nickname for FlawBOT.")] string name)
        {
            var oldName = ctx.Client.CurrentUser.Username;
            var newName = string.IsNullOrWhiteSpace(name) ? Program.Settings.Name : name;
            await ctx.Client.UpdateCurrentUserAsync(newName).ConfigureAwait(false);
            await ctx.CreateResponseAsync($"{oldName}'s username has been changed to {newName}").ConfigureAwait(false);
        }
    }
}