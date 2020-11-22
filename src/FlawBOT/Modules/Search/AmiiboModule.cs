using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class AmiiboModule : BaseCommandModule
    {
        #region COMMAND_AMIIBO

        [Command("amiibo"), Aliases("amib")]
        [Description("Retrieve Amiibo figurine information")]
        public async Task GetAmiibo(CommandContext ctx,
            [Description("Name of the Amiibo figurine"), RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await AmiiboService.GetAmiiboDataAsync(query).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            foreach (var amiibo in results.Amiibo)
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(amiibo.Name)
                    .AddField("Amiibo Series", amiibo.AmiiboSeries, true)
                    .AddField("Game Series", amiibo.GameSeries, true)
                    .AddField(":flag_us: Release:", amiibo.ReleaseDate.American, true)
                    .AddField(":flag_jp: Release:", amiibo.ReleaseDate.Japanese, true)
                    .AddField(":flag_eu: Release:", amiibo.ReleaseDate.European, true)
                    .AddField(":flag_au: Release:", amiibo.ReleaseDate.Australian, true)
                    .WithImageUrl(amiibo.Image)
                    .WithFooter(!amiibo.Equals(results.Amiibo.Last())
                        ? "Type 'next' within 10 seconds for the next amiibo"
                        : "This is the last found amiibo on the list.")
                    .WithColor(new DiscordColor("#E70009"));
                var message = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                if (results.Amiibo.Count == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                if (!amiibo.Equals(results.Amiibo.Last()))
                    await BotServices.RemoveMessage(message).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_AMIIBO
    }
}