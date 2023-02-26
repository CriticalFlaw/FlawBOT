using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class AdminModule : BaseCommandModule
    {
        #region ACTIVITY

        [RequireOwner]
        [Hidden]
        [Command("activity")]
        [Aliases("setactivity")]
        [Description("Set FlawBOT's activity.")]
        public async Task SetBotActivity(CommandContext ctx, [Description("Name of the activity.")][RemainingText] string activity)
        {
            if (string.IsNullOrWhiteSpace(activity))
            {
                await ctx.Client.UpdateStatusAsync().ConfigureAwait(false);
                return;
            }

            var game = new DiscordActivity(activity);
            await ctx.Client.UpdateStatusAsync(game).ConfigureAwait(false);
            await ctx.RespondAsync($"{Program.Settings.Name} activity has been changed to Playing {game.Name}").ConfigureAwait(false);
        }

        #endregion ACTIVITY

        #region AVATAR

        [RequireOwner]
        [Hidden]
        [Command("avatar")]
        [Aliases("setavatar", "pfp", "photo")]
        [Description("Set FlawBOT's avatar.")]
        public async Task SetBotAvatar(CommandContext ctx, [Description("Image URL. Must be in JPG, PNG or IMG format.")] string query)
        {
            var stream = BotServices.CheckImageInput(ctx, query).Result;
            if (stream.Length <= 0) return;
            await ctx.Client.UpdateCurrentUserAsync(avatar: stream).ConfigureAwait(false);
            await ctx.RespondAsync($"{Program.Settings.Name} avatar has been updated!").ConfigureAwait(false);
        }

        #endregion AVATAR

        #region STATUS

        [RequireOwner]
        [Hidden]
        [Command("status")]
        [Aliases("setstatus", "state")]
        [Description("Set FlawBOT's status.")]
        public async Task SetBotStatus(CommandContext ctx, [Description("Activity Status. Online, Idle, DND or Offline")][RemainingText] string status)
        {
            var settings = Program.Settings;
            status ??= "ONLINE";
            switch (status.Trim().ToUpperInvariant())
            {
                case "OFF":
                case "OFFLINE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Offline).ConfigureAwait(false);
                    await BotServices.SendResponseAsync(ctx, $"{settings.Name} status has been changed to Offline").ConfigureAwait(false);
                    break;

                case "INVISIBLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Invisible).ConfigureAwait(false);
                    await BotServices.SendResponseAsync(ctx, $"{settings.Name} status has been changed to Invisible").ConfigureAwait(false);
                    break;

                case "IDLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Idle).ConfigureAwait(false);
                    await BotServices.SendResponseAsync(ctx, $"{settings.Name} status has been changed to Idle").ConfigureAwait(false);
                    break;

                case "DND":
                case "DO NOT DISTURB":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.DoNotDisturb).ConfigureAwait(false);
                    await BotServices.SendResponseAsync(ctx, $"{settings.Name} status has been changed to Do Not Disturb").ConfigureAwait(false);
                    break;

                default:
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Online).ConfigureAwait(false);
                    await BotServices.SendResponseAsync(ctx, $"{settings.Name} status has been changed to Online").ConfigureAwait(false);
                    break;
            }
        }

        #endregion STATUS

        #region USERNAME

        [RequireOwner]
        [Hidden]
        [Command("username")]
        [Aliases("setusername", "name", "setname", "nickname", "nick")]
        [Description("Set FlawBOT's username.")]
        public async Task SetBotUsername(CommandContext ctx, [Description("New nickname for FlawBOT.")][RemainingText] string name)
        {
            var oldName = ctx.Client.CurrentUser.Username;
            var newName = string.IsNullOrWhiteSpace(name) ? Program.Settings.Name : name;
            await ctx.Client.UpdateCurrentUserAsync(newName).ConfigureAwait(false);
            await BotServices.SendResponseAsync(ctx, $"{oldName}'s username has been changed to {newName}").ConfigureAwait(false);
        }

        #endregion USERNAME
    }
}