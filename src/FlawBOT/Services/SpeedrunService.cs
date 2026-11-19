using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Speedrun;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var url = string.Format(Resources.URL_Speedrun, Uri.EscapeDataString(query.Trim()));
                var response = await Http.GetStringAsync(url).ConfigureAwait(false);
                var results = JsonConvert.DeserializeObject<SpeedrunGame>(response);
                if (results?.Data is null || results.Data.Count == 0) return null;
                var result = results.Data[Random.Next(results.Data.Count)];

                // Look up the categories and extras in parallel; each one is optional.
                var categoriesLink = result.Links?.FirstOrDefault(x => x.Rel == "categories")?.Url;
                var categoriesTask = categoriesLink is null ? Task.FromResult<SpeedrunCategory>(null) : GetSpeedrunCategoryAsync(categoriesLink);
                var developersTask = GetSpeedrunExtraAsync(result.Developers, SpeedrunExtras.Developers);
                var publishersTask = GetSpeedrunExtraAsync(result.Publishers, SpeedrunExtras.Publishers);
                var platformsTask = GetSpeedrunExtraAsync(result.Platforms, SpeedrunExtras.Platforms);
                await Task.WhenAll(categoriesTask, developersTask, publishersTask, platformsTask).ConfigureAwait(false);

                var categories = (await categoriesTask)?.Data?.Select(x => $"[{x.Name}]({x.Weblink})") ?? [];
                var output = new DiscordEmbedBuilder()
                    .WithTitle(result.Names?.International)
                    .AddSafeField("Developers", await developersTask, true)
                    .AddSafeField("Publishers", await publishersTask, true)
                    .AddSafeField("Release Date", result.ReleaseDate)
                    .AddSafeField("Platforms", await platformsTask)
                    .AddSafeField("Categories", categories.JoinWithinLimit(" **|** "))
                    .WithFooter($"ID: {result.Id} - Abbreviation: {result.Abbreviation}")
                    .WithColor(new DiscordColor("#0F7A4D"));
                var thumbnail = result.Assets?.CoverLarge?.Url ?? result.Assets?.Icon?.Url;
                if (!string.IsNullOrWhiteSpace(thumbnail)) output.WithThumbnail(thumbnail);
                if (!string.IsNullOrWhiteSpace(result.WebLink)) output.WithUrl(result.WebLink);
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
        /// <param name="query">Categories link of the game</param>
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
        ///     Retrieve the names of up to three of the game's platforms, genres, developers or publishers.
        /// </summary>
        /// <param name="extrasList">Speedrun extra identifiers</param>
        /// <param name="search">Speedrun extra category</param>
        public static async Task<string> GetSpeedrunExtraAsync(List<object> extrasList, SpeedrunExtras search)
        {
            if (extrasList is null || extrasList.Count == 0) return null;
            var names = await Task.WhenAll(extrasList.Take(3).Select(async extra =>
            {
                try
                {
                    var query = string.Format(Resources.URL_Speedrun_Extras, search.ToString().ToLowerInvariant(), extra);
                    var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<SpeedrunExtra>(response)?.Data?.Name;
                }
                catch
                {
                    return null;
                }
            })).ConfigureAwait(false);
            return names.JoinOrDefault(", ", null);
        }
    }
}
