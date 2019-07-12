using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class SmashService : HttpHandler
    {
        public static async Task<SmashCharacter> GetSmashCharacterAsync(string query)
        {
            try
            {
                var results = await _http.GetStringAsync("https://test-khapi.frannsoft.com/api/characters/name/" + query + "?game=ultimate");
                return JsonConvert.DeserializeObject<SmashCharacter>(results);
            }
            catch
            {
                return null;
            }
        }
    }
}