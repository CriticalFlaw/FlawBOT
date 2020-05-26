using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Group("emoji")]
    [Aliases("emojis", "em", "e")]
    [Description("Commands for managing server emojis")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class EmojiModule : BaseCommandModule
    {
        #region COMMAND_ADD

        [Command("add")]
        [Aliases("addnew", "create")]
        [Description("Add a new server emoji through URL or as an attachment.")]
        [RequirePermissions(Permissions.ManageEmojis)]
        public async Task AddAsync(CommandContext ctx,
            [Description("Name for the emoji.")] string query,
            [Description("Image URL.")] Uri url = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query) || query.Length < 2 || query.Length > 50)
                    await BotServices.SendEmbedAsync(ctx, Resources.ERR_ROLE_NAME, EmbedType.Warning)
                        .ConfigureAwait(false);

                if (url is null)
                    if (!ctx.Message.Attachments.Any() || !Uri.TryCreate(ctx.Message.Attachments.First().Url, UriKind.Absolute, out url))
                        await BotServices.SendEmbedAsync(ctx, Resources.ERR_EMOJI_IMAGE, EmbedType.Warning)
                            .ConfigureAwait(false);

                var handler = new HttpClientHandler {AllowAutoRedirect = false};
                var http = new HttpClient(handler, true);
                var response = await http.GetAsync(url).ConfigureAwait(false);
                if (!response.Content.Headers.ContentType.MediaType.StartsWith("image/")) return;

                using (response = await http.GetAsync(url).ConfigureAwait(false))
                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    if (stream.Length >= 256000)
                        await BotServices.SendEmbedAsync(ctx, Resources.ERR_EMOJI_SIZE, EmbedType.Warning)
                            .ConfigureAwait(false);
                    var emoji = await ctx.Guild.CreateEmojiAsync(query, stream).ConfigureAwait(false);
                    await BotServices
                        .SendEmbedAsync(ctx, "Successfully added " + Formatter.Bold(emoji.Name), EmbedType.Good)
                        .ConfigureAwait(false);
                }
            }
            catch
            {
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_EMOJI_ADD, EmbedType.Error).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_ADD

        #region COMMAND_DELETE

        [Command("delete")]
        [Aliases("remove", "rm", "del")]
        [Description("Remove an existing server emoji. Note: Bots can only delete emojis they created.")]
        [RequirePermissions(Permissions.ManageEmojis)]
        public async Task DeleteAsync(CommandContext ctx,
            [Description("Server emoji to delete.")] DiscordEmoji query)
        {
            try
            {
                var emoji = await ctx.Guild.GetEmojiAsync(query.Id).ConfigureAwait(false);
                await ctx.Guild.DeleteEmojiAsync(emoji).ConfigureAwait(false);
                await BotServices
                    .SendEmbedAsync(ctx, "Successfully deleted " + Formatter.Bold(emoji.Name), EmbedType.Good)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_EMOJI, EmbedType.Missing)
                    .ConfigureAwait(false);
            }
        }

        #endregion COMMAND_DELETE

        #region COMMAND_EDIT

        [Command("modify")]
        [Aliases("e", "edit", "rename")]
        [Description("Edit the name of an existing server emoji.")]
        [RequirePermissions(Permissions.ManageEmojis)]
        public async Task ModifyAsync(CommandContext ctx,
            [Description("Emoji to rename.")] DiscordEmoji query,
            [Description("New name.")] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    await BotServices.SendEmbedAsync(ctx, Resources.ERR_EMOJI_NAME, EmbedType.Warning)
                        .ConfigureAwait(false);
                }
                else
                {
                    var emoji = await ctx.Guild.GetEmojiAsync(query.Id).ConfigureAwait(false);
                    emoji = await ctx.Guild.ModifyEmojiAsync(emoji, name).ConfigureAwait(false);
                    await BotServices.SendEmbedAsync(ctx, "Successfully renamed emoji to " + Formatter.Bold(emoji.Name),
                        EmbedType.Good).ConfigureAwait(false);
                }
            }
            catch (NotFoundException)
            {
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_EMOJI, EmbedType.Missing)
                    .ConfigureAwait(false);
            }
        }

        #endregion COMMAND_EDIT

        #region COMMAND_INFO

        [Command("info")]
        [Aliases("i")]
        [Description("Retrieve server emoji information.")]
        public async Task GetEmoji(CommandContext ctx,
            [Description("Server emoji information to retrieve.")] DiscordEmoji query)
        {
            var emoji = await ctx.Guild.GetEmojiAsync(query.Id).ConfigureAwait(false);
            var output = new DiscordEmbedBuilder()
                .WithTitle(emoji.Name)
                .WithDescription("Created By" + (emoji.User is null ? "<unknown>" : emoji.User.Username))
                .AddField("Server", emoji.Guild.Name, true)
                .AddField("Creation Date", emoji.CreationTimestamp.ToString(), true)
                .WithColor(DiscordColor.PhthaloBlue)
                .WithUrl(emoji.Url)
                .WithThumbnailUrl(emoji.Url);
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_INFO

        #region COMMAND_LIST

        [Command("list")]
        [Aliases("print", "l", "ls", "all")]
        [Description("Retrieve list of server emojis.")]
        public async Task GetEmojiList(CommandContext ctx)
        {
            var emojiList = new StringBuilder();
            foreach (var emoji in ctx.Guild.Emojis.Values.OrderBy(e => e.Name))
                emojiList.Append(emoji.Name).Append(!emoji.Equals(ctx.Guild.Emojis.Last()) ? ", " : string.Empty);

            var output = new DiscordEmbedBuilder()
                .WithTitle("Emojis available for " + ctx.Guild.Name)
                .WithDescription(emojiList.ToString())
                .WithThumbnailUrl(ctx.Guild.IconUrl)
                .WithColor(DiscordColor.PhthaloBlue);
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_LIST
    }
}