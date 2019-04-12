﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Services;
using FlawBOT.Services.Games;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Games
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class SmashModule : BaseCommandModule
    {
        #region COMMAND_SMASH

        [Command("smash")]
        [Aliases("smashbros")]
        [Description("Retrieve Smash Ultimate character information")]
        public async Task GetCharacter(CommandContext ctx,
            [Description("Name of the Smash character")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(ctx, query).Result) return;
            var results = await SmashService.GetSmashCharacterAsync(query);
            if (results == null)
                await BotServices.SendEmbedAsync(ctx, ":mag: Smash character not found or not yet available!\nSee the available characters here: http://kuroganehammer.com/Ultimate", EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.DisplayName)
                    .WithDescription($"[Attributes]({results.Related.Ultimate.Attributes}) **|** [Movements]({results.Related.Ultimate.Movements}) **|** [Moves]({results.Related.Ultimate.Moves})")
                    .WithThumbnailUrl(results.ThumbnailUrl)
                    .WithColor(new DiscordColor(results.ColorTheme))
                    .WithUrl(results.FullUrl);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_SMASH
    }
}