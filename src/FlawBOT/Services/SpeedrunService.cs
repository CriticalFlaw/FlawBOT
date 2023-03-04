using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Speedrun;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class SpeedrunService : HttpHandler
    {
        /// <summary>
        ///     Retrieve game speedrun data
        /// </summary>
        /// <param name="query">Name of the game</param>
        public static async Task<DiscordEmbed> GetSpeedrunGameAsync(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                query = string.Format(Resources.URL_Speedrun, Uri.EscapeDataString(query.Trim()));
                var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                var results = JsonConvert.DeserializeObject<SpeedrunGame>(response);
                if (results.Data.Count == 0) return null;
                var result = results.Data[random.Next(results.Data.Count)];

                // TODO: Add pagination when supported for slash commands.
                var link = result.Links.First(x => x.Rel == "categories").Url;
                var categories = GetSpeedrunCategoryAsync(link).Result;
                var category = new StringBuilder();
                if (categories != null || categories.Data.Count > 0)
                    foreach (var x in categories.Data)
                        category.Append($"[{x.Name}]({x.Weblink}) **|** ");

                var output = new DiscordEmbedBuilder()
                    .WithTitle(result.Names.International)
                    .AddField("Developers", GetSpeedrunExtraAsync(result.Developers, SpeedrunExtras.Developers).Result ?? "Unknown", true)
                    .AddField("Publishers", GetSpeedrunExtraAsync(result.Publishers, SpeedrunExtras.Publishers).Result ?? "Unknown", true)
                    .AddField("Release Date", result.ReleaseDate ?? "Unknown")
                    .AddField("Platforms", GetSpeedrunExtraAsync(result.Platforms, SpeedrunExtras.Platforms).Result ?? "Unknown")
                    .WithFooter($"ID: {result.Id} - Abbreviation: {result.Abbreviation}")
                    .WithThumbnail(result.Assets.CoverLarge.Url ?? result.Assets.Icon.Url)
                    .AddField("Categories", category.ToString())
                    .WithUrl(result.WebLink)
                    .WithColor(new DiscordColor("#0F7A4D"));
                return output.Build();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Retrieve category speedrun data
        /// </summary>
        /// <param name="query">Name of the game</param>
        public static async Task<SpeedrunCategory> GetSpeedrunCategoryAsync(string query)
        {
            try
            {
                var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<SpeedrunCategory>(response);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Retrieve game's identification key for Speedrun.com
        /// </summary>
        public static async Task<string> GetSpeedrunGameIdAsync(string query)
        {
            try
            {
                query = string.Format(Resources.URL_Speedrun, Uri.EscapeDataString(query.Trim()));
                var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<SpeedrunGame>(response)?.Data.First().Id;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Retrieve the speedrun game's platforms, genres, developers or publishers.
        /// </summary>
        /// <param name="queryList">Speedrun extra identifier</param>
        /// <param name="search">Speedrun extra category</param>
        public static async Task<string> GetSpeedrunExtraAsync(List<object> extrasList, SpeedrunExtras search)
        {
            try
            {
                if (extrasList.Count == 0) return null;
                var results = new StringBuilder();
                foreach (var extra in extrasList.Take(3))
                {
                    var query = string.Format(Resources.URL_Speedrun_Extras, search.ToString().ToLowerInvariant(), extra);
                    var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                    var result = JsonConvert.DeserializeObject<SpeedrunExtra>(response)?.Data.Name;
                    results.Append(result).Append(!query.Equals(extrasList.Take(3).Last()) ? ", " : string.Empty);
                }

                return results.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}