using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Services;
using FlawBOT.Services.Search;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
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
            if (results.stream == null)
                await BotServices.SendEmbedAsync(ctx, "Twitch channel not found or it's offline", EmbedType.Missing);
            else
            {
                var stream = results.stream;
                stream.game = (string.IsNullOrWhiteSpace(stream.game)) ? "Nothing" : stream.game;
                var output = new DiscordEmbedBuilder()
                    .WithTitle(stream.channel.name + " is now live on Twitch!")
                    .WithDescription(stream.channel.status)
                    .AddField("Now Playing", stream.game)
                    .AddField("Start Time", stream.created_at.ToString(), true)
                    .AddField("Viewers", $"{stream.viewers:#,##0}", true)
                    .WithThumbnailUrl(stream.channel.logo)
                    .WithUrl(stream.channel.url)
                    .WithColor(new DiscordColor("#6441A5"));
                await ctx.RespondAsync(embed: output.Build());
                await ctx.RespondAsync(stream.channel.url);
            }
        }

        #endregion COMMAND_TWITCH
    }
}