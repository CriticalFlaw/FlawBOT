using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlawBOT.Models;
using FlawBOT.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Services.Games
{
    public class SpeedrunService : HttpHandler
    {
        /// <summary>
        ///     Retrieve game speedrun data
        /// </summary>
        /// <param name="query">Name of the game</param>
        public static async Task<SpeedrunGame> GetSpeedrunGameAsync(string query)
        {
            try
            {
                var results = await Http
                    .GetStringAsync(string.Format(Resources.URL_Speedrun, Uri.EscapeDataString(query.Trim())))
                    .ConfigureAwait(false);
                return JsonConvert.DeserializeObject<SpeedrunGame>(results);
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
                var results = await Http.GetStringAsync(query).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<SpeedrunCategory>(results);
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
                var results = await Http
                    .GetStringAsync(string.Format(Resources.URL_Speedrun, Uri.EscapeDataString(query.Trim())))
                    .ConfigureAwait(false);
                return JsonConvert.DeserializeObject<SpeedrunGame>(results)?.Data.First().Id;
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
        public static async Task<string> GetSpeedrunExtraAsync(List<object> queryList, SpeedrunExtras search)
        {
            try
            {
                if (queryList.Count == 0) return null;
                var results = new StringBuilder();
                foreach (var query in queryList.Take(3))
                {
                    var output = await Http
                        .GetStringAsync(string.Format(Resources.URL_Speedrun_Extras,
                            search.ToString().ToLowerInvariant(), query))
                        .ConfigureAwait(false);
                    var name = JsonConvert.DeserializeObject<SpeedrunExtra>(output)?.Data.Name;
                    results.Append(name).Append(!query.Equals(queryList.Take(3).Last()) ? ", " : string.Empty);
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