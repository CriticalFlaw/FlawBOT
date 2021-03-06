﻿using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class NasaModule : BaseCommandModule
    {
        #region COMMAND_NASA

        [Command("nasa")]
        [Aliases("apod", "space")]
        [Description("Retrieve NASA's Astronomy Picture of the Day.")]
        public async Task Nasa(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var results = await NasaService.GetNasaImageAsync(Program.Settings.Tokens.NasaToken).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_API_CONNECTION, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            var output = new DiscordEmbedBuilder()
                .WithDescription(results.Title)
                .WithImageUrl(results.ImageHd ?? results.ImageSd)
                .WithFooter(results.Description)
                .WithColor(new DiscordColor("#0B3D91"));
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_NASA
    }
}