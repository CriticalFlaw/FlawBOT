using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class SmashModule : BaseCommandModule
    {
        #region COMMAND_SMASH

        [Command("smash")]
        [Aliases("smashbros", "sb", "sbu")]
        [Description("Retrieve Smash Bros. Ultimate character information")]
        public async Task GetCharacter(CommandContext ctx,
            [Description("Name of the Smash character")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = await SmashService.GetSmashCharacterAsync(query);
            if (results == null)
                await BotServices.SendEmbedAsync(ctx, "Smash character not found or not yet available!\nSee the available characters here: http://kuroganehammer.com/Ultimate", EmbedType.Missing);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.DisplayName)
                    .WithThumbnailUrl(results.ThumbnailUrl)
                    .WithColor(new DiscordColor(results.ColorTheme))
                    .WithUrl(results.FullUrl);

                var attributes = await SmashService.GetCharacterAttributesAsync(results.OwnerId);
                var attributesProcessed = new List<string>();
                foreach (var attribute in attributes)
                {
                    if (attributesProcessed.Contains(attribute.Name)) continue;
                    var values = new StringBuilder();
                    foreach (var value in attribute.Attributes)
                        values.Append(value.Name + ": " + value.Value + "\n");
                    output.AddField(attribute.Name, values.ToString() ?? "Unknown", true);
                    attributesProcessed.Add(attribute.Name);
                }
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_SMASH
    }
}