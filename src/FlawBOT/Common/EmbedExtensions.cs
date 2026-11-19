using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace FlawBOT.Common
{
    /// <summary>
    ///     Helpers for building Discord embeds without tripping over Discord's field length limits.
    /// </summary>
    public static partial class EmbedExtensions
    {
        public const int FieldNameLimit = 256;
        public const int FieldValueLimit = 1024;
        public const int DescriptionLimit = 4096;
        public const int FooterLimit = 2048;
        public const int TitleLimit = 256;
        public const int MessageLimit = 2000;
        public const string Unknown = "Unknown";

        /// <summary>
        ///     Shortens text to the given length, appending an ellipsis when cut.
        /// </summary>
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength) return value;
            return maxLength <= 3 ? value[..maxLength] : value[..(maxLength - 3)] + "...";
        }

        /// <summary>
        ///     Removes HTML tags and decodes HTML entities.
        /// </summary>
        public static string StripHtml(this string value)
        {
            return string.IsNullOrEmpty(value) ? value : WebUtility.HtmlDecode(HtmlTagRegex().Replace(value, string.Empty)).Trim();
        }

        /// <summary>
        ///     Joins a list of values, skipping blanks, and falls back to a placeholder when nothing remains.
        /// </summary>
        public static string JoinOrDefault(this IEnumerable<string> values, string separator = ", ", string fallback = Unknown)
        {
            var result = string.Join(separator, values?.Where(x => !string.IsNullOrWhiteSpace(x)) ?? []);
            return string.IsNullOrWhiteSpace(result) ? fallback : result;
        }

        /// <summary>
        ///     Adds a field, substituting a placeholder for blank values and truncating values that exceed Discord's limits.
        ///     DSharpPlus throws when a field name or value is empty or too long, which previously caused whole commands to fail.
        /// </summary>
        public static DiscordEmbedBuilder AddSafeField(this DiscordEmbedBuilder builder, string name, string value, bool inline = false)
        {
            if (builder.Fields.Count >= 25) return builder;
            name = string.IsNullOrWhiteSpace(name) ? "​" : name.Truncate(FieldNameLimit);
            value = string.IsNullOrWhiteSpace(value) ? Unknown : value.Truncate(FieldValueLimit);
            return builder.AddField(name, value, inline);
        }

        /// <summary>
        ///     Joins items for a field value, dropping whole items (rather than cutting one in half) once the field is full.
        /// </summary>
        public static string JoinWithinLimit(this IEnumerable<string> items, string separator, int limit = FieldValueLimit)
        {
            var result = string.Empty;
            foreach (var item in items)
            {
                var next = result.Length == 0 ? item : result + separator + item;
                if (next.Length > limit) break;
                result = next;
            }

            return result;
        }

        [GeneratedRegex("<[^>]*>")]
        private static partial Regex HtmlTagRegex();
    }
}
