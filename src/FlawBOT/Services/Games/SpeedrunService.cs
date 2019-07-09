using FlawBOT.Common;
using FlawBOT.Models.Games;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FlawBOT.Services.Games
{
    public class SpeedrunService : HttpHandler
    {
        /// <summary>
        /// Search for a game speedrun data
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
        /// Search for a game's platform
        /// </summary>
        /// <param name="query">Name of the platform</param>
        public static async Task<SpeedrunPlatform> GetGamePlatformAsync(string query)
        {
            try
            {
                var results = await _http.GetStringAsync("https://www.speedrun.com/api/v1/platforms/" + query);
                return JsonConvert.DeserializeObject<SpeedrunPlatform>(results);
            }
            catch
            {
                return null;
            }
        }
    }
}