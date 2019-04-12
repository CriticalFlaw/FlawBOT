using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Models;
using FlawBOT.Services;
using FlawBOT.Services.Games;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Bot
{
    [Group("sudo"), Hidden]
    [Description("Owner commands for controlling FlawBOT")]
    [Cooldown(3, 5, CooldownBucketType.Global)]
    public class OwnerModule : BaseCommandModule
    {
        #region COMMAND_ACTIVITY

        [RequireOwner]
        [Command("activity"), Hidden]
        [Aliases("setactivity")]
        [Description("Set FlawBOT's activity")]
        public async Task SetBotActivity(CommandContext ctx,
            [Description("Name of the activity")] [RemainingText] string activity)
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

        #endregion COMMAND_ACTIVITY

        #region COMMAND_AVATAR

        [RequireOwner]
        [Command("avatar"), Hidden]
        [Aliases("setavatar")]
        [Description("Set FlawBOT's avatar")]
        public async Task SetBotAvatar(CommandContext ctx, 
            [Description("Avatar URL. Must be in jpg, png or img format.")] string query)
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

        #region COMMAND_SHUTDOWN

        [RequireOwner]
        [Command("shutdown"), Hidden]
        [Aliases("off", "exit")]
        [Description("Shutdown the FlawBOT client")]
        public async Task Shutdown(CommandContext ctx)
        {
            await BotServices.SendEmbedAsync(ctx, "Are you sure you want shutdown FlawBOT?\nRespond with **yes** to proceed or wait 10 seconds to cancel this operation.");
            var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "yes", TimeSpan.FromSeconds(10));
            if (interactivity != null)
            {
                await BotServices.SendEmbedAsync(ctx, "Shutting down FlawBOT...");
                await Task.Delay(0).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_SHUTDOWN

        #region COMMAND_STATUS

        [RequireOwner]
        [Command("status"), Hidden]
        [Aliases("setstatus")]
        [Description("Set FlawBOT's status")]
        public async Task SetBotStatus(CommandContext ctx,
            [Description("Activity Status. Online, Idle, DND or Offline")] [RemainingText] string status)
        {
            status = (string.IsNullOrWhiteSpace(status)) ? "ONLINE" : status;
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

        #endregion COMMAND_STATUS

        #region COMMAND_UPDATE

        [RequireOwner]
        [Command("update"), Hidden]
        [Aliases("refresh")]
        [Description("Update FlawBOT libraries")]
        public async Task Update(CommandContext ctx)
        {
            var message = await ctx.RespondAsync("Starting update...");
            await BotServices.UpdateSteamAsync();
            await PokemonService.GetPokemonDataAsync();
            await message.ModifyAsync("Starting update...Done!");
        }

        #endregion COMMAND_UPDATE
    }
}