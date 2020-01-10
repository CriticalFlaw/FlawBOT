using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            var results = await SmashService.GetSmashCharacterAsync(query).ConfigureAwait(false);
            if (results is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_SMASH, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.DisplayName)
                    .WithThumbnailUrl(results.ThumbnailUrl)
                    .WithColor(new DiscordColor(results.ColorTheme))
                    .WithUrl(results.FullUrl);

                var attributes = await SmashService.GetCharacterAttributesAsync(results.OwnerId).ConfigureAwait(false);
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
                await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_SMASH
    }
}