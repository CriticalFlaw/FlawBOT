using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class SpeedrunService : HttpHandler
    {
        /// <summary>
        /// Retrieve game speedrun data
        /// </summary>
        /// <param name="query">Name of the game</param>
        public static async Task<SpeedrunGame> GetSpeedrunGameAsync(string query)
        {
            try
            {
                var results = await _http.GetStringAsync("https://www.speedrun.com/api/v1/games?name=" + query + "&max=1");
                return JsonConvert.DeserializeObject<SpeedrunGame>(results);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieve the speedrun game's platforms, genres, developers or publishers.
        /// </summary>
        /// <param name="queryList">Developer IDs</param>
        public static async Task<string> GetSpeedrunExtraAsync(List<object> queryList, SpeedrunExtras search)
        {
            try
            {
                var results = new StringBuilder();
                foreach (var query in queryList)
                {
                    var output = await _http.GetStringAsync("https://www.speedrun.com/api/v1/" + search.ToString().ToLowerInvariant() + "/" + query);
                    var name = JsonConvert.DeserializeObject<SpeedrunExtra>(output).Data.Name;
                    results.Append(name + "\n");
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