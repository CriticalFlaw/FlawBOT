using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class BotServices
    {
        public static async Task SendResponseAsync(CommandContext ctx, string message, ResponseType type = ResponseType.Default)
        {
            message = type switch
            {
                ResponseType.Warning => ":exclamation: " + message,
                ResponseType.Missing => ":mag: " + message,
                ResponseType.Error => ":no_entry: " + message,
                _ => message
            };

            await ctx.RespondAsync(message).ConfigureAwait(false);
        }
        public static async Task SendResponseAsync(InteractionContext ctx, string message, ResponseType type = ResponseType.Default)
        {
            message = type switch
            {
                ResponseType.Warning => ":exclamation: " + message,
                ResponseType.Missing => ":mag: " + message,
                ResponseType.Error => ":no_entry: " + message,
                _ => message
            };

            await ctx.CreateResponseAsync(message).ConfigureAwait(false);
        }

        public static async Task SendUserStateChangeAsync(CommandContext ctx, UserStateChange state, DiscordMember user,
            string reason)
        {
            var output = new DiscordEmbedBuilder()
                .WithDescription(
                    $"{state}: {user.DisplayName}#{user.Discriminator}\nIdentifier: {user.Id}\nReason: {reason}\nIssued by: {ctx.Member.DisplayName}#{ctx.Member.Discriminator}")
                .WithColor(DiscordColor.Green);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        public static bool CheckChannelName(string input)
        {
            return !string.IsNullOrWhiteSpace(input) && input.Length <= 100;
        }

        public static async Task<InteractivityResult<DiscordMessage>> GetUserInteractivity(CommandContext ctx,
            string keyword, int seconds)
        {
            return await ctx.Client.GetInteractivity()
                .WaitForMessageAsync(
                    m => m.Channel.Id == ctx.Channel.Id &&
                         string.Equals(m.Content, keyword, StringComparison.InvariantCultureIgnoreCase),
                    TimeSpan.FromSeconds(seconds)).ConfigureAwait(false);
        }

        public static async Task<InteractivityResult<DiscordMessage>> GetUserInteractivity(InteractionContext ctx,
            string keyword, int seconds)
        {
            return await ctx.Client.GetInteractivity()
                .WaitForMessageAsync(
                    m => m.Channel.Id == ctx.Channel.Id &&
                         string.Equals(m.Content, keyword, StringComparison.InvariantCultureIgnoreCase),
                    TimeSpan.FromSeconds(seconds)).ConfigureAwait(false);
        }

        public static int LimitToRange(int value, int min = 1, int max = 100)
        {
            if (value <= min) return min;
            return value >= max ? max : value;
        }

        public static async Task RemoveMessage(DiscordMessage message)
        {
            await message.DeleteAsync().ConfigureAwait(false);
        }

        public static async Task<MemoryStream> CheckImageInput(CommandContext ctx, string input)
        {
            var stream = new MemoryStream();
            if (input != null && !Uri.TryCreate(input, UriKind.Absolute, out _) &&
                (!input.EndsWith(".img") || !input.EndsWith(".png") || !input.EndsWith(".jpg")))
            {
                await SendResponseAsync(ctx, Resources.URL_INVALID_IMG, ResponseType.Warning)
                    .ConfigureAwait(false);
            }
            else
            {
                using HttpClient client = new();
                using var httpResponse = await client.GetAsync(input).ConfigureAwait(false);
                var result = await httpResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                stream.Write(result, 0, result.Length);
                stream.Position = 0;
            }

            return stream;
        }

        public static async Task<MemoryStream> CheckImageInput(InteractionContext ctx, string input)
        {
            var stream = new MemoryStream();
            if (input != null && !Uri.TryCreate(input, UriKind.Absolute, out _) &&
                (!input.EndsWith(".img") || !input.EndsWith(".png") || !input.EndsWith(".jpg")))
            {
                await SendResponseAsync(ctx, Resources.URL_INVALID_IMG, ResponseType.Warning)
                    .ConfigureAwait(false);
            }
            else
            {
                using HttpClient client = new();
                using var httpResponse = await client.GetAsync(input).ConfigureAwait(false);
                var result = await httpResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                stream.Write(result, 0, result.Length);
                stream.Position = 0;
            }

            return stream;
        }
    }
}