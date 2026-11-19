using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class BotServices : HttpHandler
    {
        /// <summary>
        ///     Largest image accepted for avatar and server icon uploads (Discord caps these at 10MB).
        /// </summary>
        private const long MaxImageBytes = 10 * 1024 * 1024;

        public static string FormatResponse(string message, ResponseType type = ResponseType.Default)
        {
            message = type switch
            {
                ResponseType.Warning => ":exclamation: " + message,
                ResponseType.Missing => ":mag: " + message,
                ResponseType.Error => ":no_entry: " + message,
                _ => message
            };
            return message.Truncate(EmbedExtensions.MessageLimit);
        }

        /// <summary>
        ///     Responds to an interaction with a text message. If the interaction was already acknowledged
        ///     (deferred or answered), the message is sent as a follow-up instead of failing.
        /// </summary>
        public static async Task SendResponseAsync(InteractionContext ctx, string message, ResponseType type = ResponseType.Default)
        {
            message = FormatResponse(message, type);
            try
            {
                await ctx.CreateResponseAsync(message).ConfigureAwait(false);
            }
            catch (BadRequestException)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(message)).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Completes a deferred interaction with either the embed or, when it is null, a "not found" style message.
        /// </summary>
        public static async Task SendDeferredAsync(InteractionContext ctx, DiscordEmbed output, string missingMessage = null, ResponseType missingType = ResponseType.Missing, string content = null)
        {
            var builder = new DiscordWebhookBuilder();
            if (output is null)
                builder.WithContent(FormatResponse(missingMessage ?? Resources.NOT_FOUND_COMMON, missingType));
            else
            {
                builder.AddEmbed(output);
                if (!string.IsNullOrWhiteSpace(content)) builder.WithContent(content);
            }

            await ctx.EditResponseAsync(builder).ConfigureAwait(false);
        }

        /// <summary>
        ///     Completes a deferred interaction with a text message.
        /// </summary>
        public static Task SendDeferredAsync(InteractionContext ctx, string message, ResponseType type = ResponseType.Default)
        {
            return ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(FormatResponse(message, type)));
        }

        public static bool CheckChannelName(string input)
        {
            return !string.IsNullOrWhiteSpace(input) && input.Trim().Length <= 100;
        }

        public static int LimitToRange(double value, int min = 1, int max = 100)
        {
            if (double.IsNaN(value) || value <= min) return min;
            return (int)(value >= max ? max : value);
        }

        /// <summary>
        ///     Validates that the input is an http(s) URL.
        /// </summary>
        public static bool IsWebUrl(string input, out Uri uri)
        {
            return Uri.TryCreate(input?.Trim(), UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
        }

        /// <summary>
        ///     Downloads an image from a URL. Returns null when the URL is invalid, unreachable, not an image or too large.
        /// </summary>
        public static async Task<MemoryStream> CheckImageInput(string input)
        {
            if (!IsWebUrl(input, out var uri)) return null;
            try
            {
                using var response = await Http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode) return null;
                if (response.Content.Headers.ContentType?.MediaType?.StartsWith("image/", StringComparison.OrdinalIgnoreCase) != true) return null;
                if (response.Content.Headers.ContentLength > MaxImageBytes) return null;

                var bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                if (bytes.Length == 0 || bytes.Length > MaxImageBytes) return null;
                return new MemoryStream(bytes);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return null;
            }
        }

        public static string GetCurrentUptime()
        {
            return FormatUptime(DateTime.Now - Program.Settings.ProcessStarted);
        }

        public static string FormatUptime(TimeSpan uptime)
        {
            var days = uptime.Days > 0 ? $" ({uptime.Days} day{(uptime.Days == 1 ? "" : "s")})" : string.Empty;
            return $"{uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}{days}";
        }

        /// <summary>
        ///     Formats a user name, including the legacy discriminator only for accounts that still have one.
        /// </summary>
        public static string FormatUserName(DiscordUser user)
        {
            if (user is null) return EmbedExtensions.Unknown;
            return string.IsNullOrEmpty(user.Discriminator) || user.Discriminator == "0" ? user.Username : $"{user.Username}#{user.Discriminator}";
        }
    }
}
