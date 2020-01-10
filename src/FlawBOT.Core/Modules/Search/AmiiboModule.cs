using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class AmiiboModule : BaseCommandModule
    {
        #region COMMAND_AMIIBO

        [Command("amiibo")]
        [Aliases("amib")]
        [Description("Retrieve Amiibo figurine information")]
        public async Task GetAmiibo(CommandContext ctx,
            [Description("Name of the Amiibo figurine")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = await AmiiboService.GetAmiiboFigurineAsync(query).ConfigureAwait(false);
            if (results is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
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
                        .WithFooter((results.Amiibo.Count > 1) ? "Type 'next' within 10 seconds for the next amiibo" : "This is the only amiibo of this name")
                        .WithColor(new DiscordColor("#E70009"));
                    var message = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                    if (results.Amiibo.Count == 1) continue;
                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                    if (interactivity.Result is null) break;
                    await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                    await BotServices.RemoveMessage(message).ConfigureAwait(false);
                }
            }
        }

        #endregion COMMAND_AMIIBO
    }
}