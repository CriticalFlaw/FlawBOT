﻿using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
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
                await ctx.Client.UpdateStatusAsync(activity: null);
                await BotServices.SendEmbedAsync(ctx, SharedData.Name + " activity has been changed to " + Formatter.Bold("Normal"));
            }
            else
            {
                // TODO: Set the activity type
                var game = new DiscordActivity(activity);
                await ctx.Client.UpdateStatusAsync(activity: game);
                await BotServices.SendEmbedAsync(ctx, SharedData.Name + " activity has been changed to " + Formatter.Bold("Playing " + game.Name), EmbedType.Good);
            }
        }

        #endregion COMMAND_ACTIVITY

        #region COMMAND_AVATAR

        [RequireOwner]
        [Command("avatar"), Hidden]
        [Aliases("setavatar")]
        [Description("Set FlawBOT's avatar")]
        public async Task SetBotAvatar(CommandContext ctx,
            [Description("Image URL. Must be in jpg, png or img format.")] string query)
        {
            var stream = BotServices.CheckImageInput(ctx, query).Result;
            if (stream.Length <= 0) return;
            await ctx.Client.UpdateCurrentUserAsync(avatar: stream);
            await BotServices.SendEmbedAsync(ctx, SharedData.Name + " avatar has been updated!", EmbedType.Good);
        }

        #endregion COMMAND_AVATAR

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
                    await BotServices.SendEmbedAsync(ctx, SharedData.Name + " status has been changed to **Offline**");
                    break;

                case "INVISIBLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Invisible);
                    await BotServices.SendEmbedAsync(ctx, SharedData.Name + " status has been changed to **Invisible**");
                    break;

                case "IDLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Idle);
                    await BotServices.SendEmbedAsync(ctx, SharedData.Name + " status has been changed to **Idle**", EmbedType.Warning);
                    break;

                case "DND":
                case "DO NOT DISTURB":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.DoNotDisturb);
                    await BotServices.SendEmbedAsync(ctx, SharedData.Name + " status has been changed to **Do Not Disturb**", EmbedType.Error);
                    break;

                default:
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Online);
                    await BotServices.SendEmbedAsync(ctx, SharedData.Name + " status has been changed to **Online**", EmbedType.Good);
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
            await SteamService.UpdateSteamListAsync().ConfigureAwait(false);
            await TeamFortressService.LoadTF2SchemaAsync().ConfigureAwait(false);
            await PokemonService.UpdatePokemonListAsync().ConfigureAwait(false);
            await message.ModifyAsync("Starting update...done!");
        }

        #endregion COMMAND_UPDATE

        #region COMMAND_USERNAME

        [RequireOwner]
        [Command("username"), Hidden]
        [Aliases("setusername", "name", "setname", "nickname")]
        [Description("Set FlawBOT's username")]
        public async Task SetBotUsername(CommandContext ctx,
            [Description("New bot username")] [RemainingText] string name)
        {
            var oldUsername = ctx.Client.CurrentUser.Username;
            if (string.IsNullOrWhiteSpace(name))
            {
                await ctx.Client.UpdateCurrentUserAsync(username: SharedData.Name);
                await BotServices.SendEmbedAsync(ctx, oldUsername + " username has been changed to " + SharedData.Name);
            }
            else
            {
                await ctx.Client.UpdateCurrentUserAsync(username: name);
                await BotServices.SendEmbedAsync(ctx, oldUsername + " username has been changed to " + ctx.Client.CurrentUser.Username, EmbedType.Good);
            }
        }

        #endregion COMMAND_USERNAME
    }
}