using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class NASAModule : BaseCommandModule
    {
        #region COMMAND_NASA

        [Command("nasa")]
        [Aliases("apod", "space")]
        [Description("Retrieve NASA's Astronomy Picture of the Day")]
        public async Task NASA(CommandContext ctx)
        {
            var results = await NASAService.GetNASAImageAsync().ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_NASA_API, EmbedType.Missing).ConfigureAwait(false);
            }
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithDescription(results.Title)
                    .WithImageUrl(results.ImageHD ?? results.ImageSD)
                    .WithFooter(results.Description)
                    .WithColor(new DiscordColor("#0B3D91"));
                await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_NASA
    }
}