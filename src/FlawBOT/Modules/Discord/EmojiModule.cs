using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using FlawBOT.Common;
using FlawBOT.Modules.Bot;
using FlawBOT.Properties;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Discord
{
    [Group("emoji")]
    [Aliases("emote")]
    [Description("Commands for managing server emojis.")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class EmojiModule : BaseCommandModule
    {
        #region COMMAND_ADD

        [Command("create")]
        [Aliases("new", "add")]
        [Description("Add a new server emoji using a URL image.")]
        [RequirePermissions(Permissions.ManageEmojis)]
        public async Task CreateEmoji(CommandContext ctx,
            [Description("Image URL.")] Uri url,
            [Description("Name for the emoji.")] [RemainingText]
            string name)
        {
            try
            {
                if (url is null)
                {
                    if (!ctx.Message.Attachments.Any() ||
                        !Uri.TryCreate(ctx.Message.Attachments[0].Url, UriKind.Absolute, out url))
                        await BotServices.SendResponseAsync(ctx, Resources.ERR_EMOJI_IMAGE, ResponseType.Warning)
                            .ConfigureAwait(false);
                    return;
                }

                if (string.IsNullOrWhiteSpace(name) || name.Length < 2 || name.Length > 50)
                {
                    await BotServices.SendResponseAsync(ctx, Resources.ERR_EMOJI_NAME, ResponseType.Warning)
                        .ConfigureAwait(false);
                    return;
                }

                var handler = new HttpClientHandler { AllowAutoRedirect = false };
                var http = new HttpClient(handler, true);
                var response = await http.GetAsync(url).ConfigureAwait(false);
                if (!response.Content.Headers.ContentType.MediaType.StartsWith("image/")) return;

                using (response = await http.GetAsync(url).ConfigureAwait(false))
                await using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    if (stream.Length >= 256000)
                    {
                        await BotServices.SendResponseAsync(ctx, Resources.ERR_EMOJI_SIZE, ResponseType.Warning)
                            .ConfigureAwait(false);
                        return;
                    }

                    var emoji = await ctx.Guild.CreateEmojiAsync(name, stream).ConfigureAwait(false);
                    await ctx.RespondAsync("Created the emoji " + Formatter.Bold(emoji.Name)).ConfigureAwait(false);
                }
            }
            catch
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_EMOJI_ADD, ResponseType.Error)
                    .ConfigureAwait(false);
            }
        }

        #endregion COMMAND_ADD

        #region COMMAND_DELETE

        [Command("delete")]
        [Aliases("remove")]
        [Description("Remove a server emoji. Note: Bots can only delete emojis they created.")]
        [RequirePermissions(Permissions.ManageEmojis)]
        public async Task DeleteEmoji(CommandContext ctx,
            [Description("Server emoji to delete.")]
            DiscordEmoji query)
        {
            try
            {
                var emoji = await ctx.Guild.GetEmojiAsync(query.Id).ConfigureAwait(false);
                await ctx.Guild.DeleteEmojiAsync(emoji).ConfigureAwait(false);
                await ctx.RespondAsync("Deleted the emoji " + Formatter.Bold(emoji.Name)).ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_EMOJI, ResponseType.Missing)
                    .ConfigureAwait(false);
            }
        }

        #endregion COMMAND_DELETE

        #region COMMAND_EDIT

        [Command("rename")]
        [Aliases("edit", "modify")]
        [Description("Rename a server emoji.")]
        [RequirePermissions(Permissions.ManageEmojis)]
        public async Task EditEmoji(CommandContext ctx,
            [Description("Emoji to rename.")] DiscordEmoji query,
            [Description("New emoji name.")] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    await BotServices.SendResponseAsync(ctx, Resources.ERR_EMOJI_NAME, ResponseType.Warning)
                        .ConfigureAwait(false);
                    return;
                }

                var emoji = await ctx.Guild.GetEmojiAsync(query.Id).ConfigureAwait(false);
                emoji = await ctx.Guild.ModifyEmojiAsync(emoji, name).ConfigureAwait(false);
                await ctx.RespondAsync("Successfully renamed emoji to " + Formatter.Bold(emoji.Name))
                    .ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_EMOJI, ResponseType.Missing)
                    .ConfigureAwait(false);
            }
        }

        #endregion COMMAND_EDIT

        #region COMMAND_INFO

        [Command("info")]
        [Description("Retrieve server emoji information.")]
        public async Task GetEmojiInfo(CommandContext ctx,
            [Description("Server emoji.")] DiscordEmoji query)
        {
            var emoji = await ctx.Guild.GetEmojiAsync(query.Id).ConfigureAwait(false);
            var output = new DiscordEmbedBuilder()
                .WithDescription(emoji.Name + " (" + emoji.Guild.Name + ")")
                .AddField("Created by",
                    (emoji.User is null ? "<unknown>" : emoji.User.Username) + " on " + emoji.CreationTimestamp.Date)
                .WithColor(DiscordColor.PhthaloBlue)
                .WithThumbnail(emoji.Url);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_INFO

        #region COMMAND_LIST

        [Command("list")]
        [Aliases("print", "all")]
        [Description("Retrieve a list of server emojis.")]
        public async Task GetEmojiList(CommandContext ctx)
        {
            var emojiList = new StringBuilder();
            foreach (var emoji in ctx.Guild.Emojis.Values)
                emojiList.Append(emoji.Name).Append(!emoji.Equals(ctx.Guild.Emojis.Last().Value) ? ", " : string.Empty);

            var output = new DiscordEmbedBuilder()
                .WithTitle(ctx.Guild.Name + " Emoji List")
                .WithDescription(emojiList.ToString())
                .WithThumbnail(ctx.Guild.IconUrl)
                .WithColor(DiscordColor.PhthaloBlue);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_LIST
    }
}