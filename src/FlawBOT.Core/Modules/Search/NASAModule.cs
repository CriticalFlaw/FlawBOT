using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class NASAModule : BaseCommandModule
    {
        #region COMMAND_NASA

        [Command("nasa")]
        [Aliases("apod")]
        [Description("Retrieve NASA's Astronomy Picture of the Day")]
        public async Task NASA(CommandContext ctx)
        {
            var results = await NASAService.GetNASAImage();
            if (results == null)
                await BotServices.SendEmbedAsync(ctx, "Unable to reach NASA API", EmbedType.Missing);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.Title)
                    .WithDescription(results.Description)
                    .WithImageUrl(results.ImageHD ?? results.ImageSD)
                    .WithFooter(results.Copyright + " - " + results.Date)
                    .WithColor(new DiscordColor("#0B3D91"));
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_NASA
    }
}