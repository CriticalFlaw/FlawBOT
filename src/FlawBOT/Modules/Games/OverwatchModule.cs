using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Services;
using OverwatchAPI;
using OverwatchAPI.Extensions;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Games
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class OverwatchModule : BaseCommandModule
    {
        #region COMMAND_OVERWATCH

        [Command("ow")]
        [Aliases("overwatch")]
        [Description("Retrieve Overwatch player information")]
        public async Task Overwatch(CommandContext ctx,
            [Description("Player battletag. Must follow the exact format.")] string battletag,
            [Description("Display competitive stats if this parameter is 'comp'")] [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(battletag))
                await BotServices.SendEmbedAsync(ctx, ":warning: Battletag is required! Try **.ow CriticalFlaw#11354** (case-sensitive)", EmbedType.Warning);
            else
            {
                var overwatch = new OverwatchClient();
                var player = await overwatch.GetPlayerAsync(battletag);
                if (player == null)
                    await BotServices.SendEmbedAsync(ctx, ":mag: Player not found! Remember, battletags are case-sensitive.", EmbedType.Warning);
                else
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(player.Username)
                        .AddField("Level", player.PlayerLevel.ToString(), true)
                        .AddField("Competitive", player.CompetitiveRank.ToString(), true)
                        .AddField("Platform", player.Platform.ToString().ToUpperInvariant(), true)
                        .WithThumbnailUrl(player.ProfilePortraitUrl)
                        .WithUrl(player.ProfileUrl)
                        .WithColor(new DiscordColor("#F99E1A"));
                    if (player.IsProfilePrivate) output.WithFooter("This profile is private. No other data can be displayed");
                    else
                    {
                        if (player.Achievements != null) output.AddField("Achievements", player.Achievements.Count.ToString(), true);
                        // Add Casual or Competitive stats if a query is present
                        switch (query.ToUpperInvariant())
                        {
                            case "CASUAL":
                            default:
                                if (player.CasualStats != null) output.AddField("Healing Done", player.CasualStats.GetStatExact("All Heroes", "Assists", "Healing Done").Value.ToString(), true);
                                output.WithFooter("Casual stats shown are for All Heroes");
                                break;

                            case "COMP":
                            case "COMPETITIVE":
                                if (player.CompetitiveStats != null) output.AddField("Healing Done", player.CompetitiveStats.GetStatExact("All Heroes", "Assists", "Healing Done").Value.ToString(), true);
                                output.WithFooter("Competitive stats shown are for All Heroes");
                                break;
                        }
                    }
                    await ctx.RespondAsync(embed: output.Build());
                }
            }
        }

        #endregion COMMAND_OVERWATCH
    }
}