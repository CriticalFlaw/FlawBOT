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
        public static async Task<Data> GetSpeedrunGameAsync(string query)
        {
            try
            {
                query = string.Format(Resources.URL_Speedrun, Uri.EscapeDataString(query.Trim()));
                var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<SpeedrunGame>(response);
                if (result.Data.Count == 0) return null;
                return result.Data[random.Next(result.Data.Count)];
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