using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Models;
using FlawBOT.Services;
using FlawBOT.Services.Search;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
{
    public class TwitchModule : BaseCommandModule
    {
        #region COMMAND_TWITCH

        [Command("twitch")]
        [Aliases("stream")]
        [Description("Retrieve Twitch stream information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Twitch(CommandContext ctx, [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(ctx, query).Result) return;
            var data = await TwitchService.GetTwitchDataAsync(query);
            if (data.stream == null)
                await BotServices.SendEmbedAsync(ctx, ":mag: Twitch channel not found or it's offline", EmbedType.Warning);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(data.stream.channel.name + " is now live on Twitch!")
                    .WithDescription(data.stream.channel.status)
                    .AddField("Now Playing", data.stream.game)
                    .AddField("Start Time", data.stream.created_at.ToString(), true)
                    .AddField("Viewers", data.stream.viewers.ToString(), true)
                    .WithThumbnailUrl(data.stream.channel.logo)
                    .WithUrl(data.stream.channel.url)
                    .WithColor(DiscordColor.Purple);
                await ctx.RespondAsync(embed: output.Build());
                await ctx.RespondAsync(data.stream.channel.url);
            }
        }

        #endregion COMMAND_TWITCH
    }
}