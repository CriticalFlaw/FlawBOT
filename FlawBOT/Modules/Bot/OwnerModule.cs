using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Bot
{
    [Group("sudo"), Hidden]
    [Cooldown(3, 5, CooldownBucketType.Global)]
    public class OwnerModule : BaseCommandModule
    {
        #region COMMAND_BOTACTIVITY

        [Hidden]
        [RequireOwner]
        [Command("setactivity")]
        [Description("Set FlawBOT's activity")]
        public async Task SetBotActivity(CommandContext ctx, [RemainingText] string activity)
        {
            if (string.IsNullOrWhiteSpace(activity))
            {
                await ctx.Client.UpdateStatusAsync(null);
                await BotServices.SendEmbedAsync(ctx, "FlawBOT activity has been changed to **Normal**");
            }
            else
            {
                var game = new DiscordActivity { Name = activity };
                await ctx.Client.UpdateStatusAsync(game);
                await BotServices.SendEmbedAsync(ctx, $"FlawBOT activity has been changed to **Playing {game.Name}**", EmbedType.Good);
            }
        }

        #endregion COMMAND_BOTACTIVITY

        #region COMMAND_AVATAR

        [Hidden]
        [RequireOwner]
        [Command("avatar")]
        [Aliases("setavatar")]
        [Description("Set FlawBOT's avatar")]
        public async Task SetBotAvatar(CommandContext ctx, string query)
        {
            var stream = BotServices.CheckImageInput(ctx, query).Result;
            try
            {
                await ctx.Client.UpdateCurrentUserAsync(avatar: stream);
                await BotServices.SendEmbedAsync(ctx, "FlawBOT avatar has been updated!", EmbedType.Good);
            }
            catch
            {
                await BotServices.SendEmbedAsync(ctx, "FlawBOT avatar has not been updated!", EmbedType.Error);
            }
        }

        #endregion COMMAND_AVATAR

        #region COMMAND_BOTSTATUS

        [Hidden]
        [RequireOwner]
        [Command("setstatus")]
        [Description("Set FlawBOT's status")]
        public async Task SetBotStatus(CommandContext ctx, [RemainingText] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Online);
            else
            {
                switch (status.Trim().ToUpperInvariant())
                {
                    case "OFF":
                    case "OFFLINE":
                        await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Offline);
                        await BotServices.SendEmbedAsync(ctx, "FlawBOT status has been changed to **Offline**");
                        break;

                    case "INVISIBLE":
                        await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Invisible);
                        await BotServices.SendEmbedAsync(ctx, "FlawBOT status has been changed to **Invisible**");
                        break;

                    case "IDLE":
                        await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Idle);
                        await BotServices.SendEmbedAsync(ctx, "FlawBOT status has been changed to **Idle**", EmbedType.Warning);
                        break;

                    case "DND":
                    case "DO NOT DISTURB":
                        await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.DoNotDisturb);
                        await BotServices.SendEmbedAsync(ctx, "FlawBOT status has been changed to **Do Not Disturb**", EmbedType.Error);
                        break;

                    default:
                        await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Online);
                        await BotServices.SendEmbedAsync(ctx, "FlawBOT status has been changed to **Online**", EmbedType.Good);
                        break;
                }
            }
        }

        #endregion COMMAND_BOTSTATUS

        #region COMMAND_SHUTDOWN

        [Hidden]
        [RequireOwner]
        [Command("shutdown")]
        [Description("Shutdown the FlawBOT client")]
        [Aliases("off", "quit", "exit")]
        public async Task Shutdown(CommandContext ctx)
        {
            await Task.Delay(0).ConfigureAwait(false);
        }

        #endregion COMMAND_SHUTDOWN

        #region COMMAND_UPDATE

        [Hidden]
        [RequireOwner]
        [Command("update")]
        [Description("Update FlawBOT libraries")]
        public async Task Update(CommandContext ctx)
        {
            await BotServices.UpdateSteamAsync();
            await BotServices.SendEmbedAsync(ctx, "Completed Steam library updates", EmbedType.Good);
        }

        #endregion COMMAND_UPDATE
    }
}