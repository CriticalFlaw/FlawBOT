using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class TwitchModule : BaseCommandModule
    {
        #region COMMAND_TWITCH

        [Command("twitch")]
        [Aliases("stream")]
        [Description("Retrieve Twitch stream information")]
        public async Task Twitch(CommandContext ctx,
            [Description("Channel to find on Twitch")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = await TwitchService.GetTwitchDataAsync(query);
            if (results.Stream == null)
                await BotServices.SendEmbedAsync(ctx, "Twitch channel not found or it's offline", EmbedType.Missing);
            else
            {
                var stream = results.Stream;
                var output = new DiscordEmbedBuilder()
                    .WithTitle(stream.Channel.Name + " is now live on Twitch!")
                    .WithDescription(stream.Channel.Status)
                    .AddField("Now Playing", (stream.Game) ?? "Nothing")
                    .AddField("Start Time", stream.CreatedAt.ToString(), true)
                    .AddField("Viewers", $"{stream.Viewers:#,##0}", true)
                    .WithThumbnailUrl(stream.Channel.Logo)
                    .WithUrl(stream.Channel.Url)
                    .WithColor(new DiscordColor("#6441A5"));
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_TWITCH
    }
}