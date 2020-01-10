using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class SmashService : HttpHandler
    {
        public static async Task<SmashCharacter> GetSmashCharacterAsync(string query)
        {
            try
            {
                var results = await _http.GetStringAsync(Resources.API_SmashBros + "name/" + query.ToLowerInvariant() + "?game=ultimate").ConfigureAwait(false);
                return JsonConvert.DeserializeObject<SmashCharacter>(results);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<List<SmashCharacterAttributes>> GetCharacterAttributesAsync(int characterID)
        {
            try
            {
                var output = await _http.GetStringAsync(Resources.API_SmashBros + characterID + "/characterattributes?game=ultimate").ConfigureAwait(false);
                var attributes = JsonConvert.DeserializeObject<List<SmashCharacterAttributes>>(output);
                return attributes.Distinct().ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}