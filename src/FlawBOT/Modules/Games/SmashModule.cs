using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Services;
using FlawBOT.Services.Games;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Games
{
    public class SmashModule : BaseCommandModule
    {
        #region COMMAND_SMASH

        [Command("smash")]
        [Description("Get Smash Ultimate character information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetCharacter(CommandContext ctx, [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(ctx, query).Result) return;
            var data = await SmashService.GetSmashCharacterAsync(query);
            if (data == null)
                await BotServices.SendEmbedAsync(ctx, ":mag: Smash character not found or not yet available! See the available characters here: http://kuroganehammer.com/Ultimate", EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(data.DisplayName)
                    .WithDescription($"[Attributes]({data.Related.Ultimate.Attributes}) **|** [Movements]({data.Related.Ultimate.Movements}) **|** [Moves]({data.Related.Ultimate.Moves})")
                    .WithThumbnailUrl(data.ThumbnailUrl)
                    .WithUrl(data.FullUrl);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_SMASH
    }
}