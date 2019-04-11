using FlawBOT.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services.Games
{
    public class SmashService
    {
        private static readonly string base_url = "https://test-khapi.frannsoft.com/api/characters/name/";
        private static readonly string attributes_url = "/characterattributes?game=ultimate";
        private static readonly string movements_url = "/movements?game=ultimate";
        private static readonly string moves_url = "/moves?game=ultimate";
        private static readonly HttpClient http = new HttpClient();

        public static async Task<SmashCharacter> GetSmashCharacterAsync(string query)
        {
            try
            {
                var data = await http.GetStringAsync(base_url + query + "?game=ultimate");
                return JsonConvert.DeserializeObject<SmashCharacter>(data);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<SmashCharacter> GetSmashAttributesAsync(string query)
        {
            var data = await http.GetStringAsync(base_url + query + attributes_url);
            return JsonConvert.DeserializeObject<SmashCharacter>(data);
        }

        public static async Task<SmashCharacter> GetSmashMovementsAsync(string query)
        {
            var data = await http.GetStringAsync(base_url + query + movements_url);
            return JsonConvert.DeserializeObject<SmashCharacter>(data);
        }

        public static async Task<SmashCharacter> GetSmashMovesAsync(string query)
        {
            var data = await http.GetStringAsync(base_url + query + moves_url);
            return JsonConvert.DeserializeObject<SmashCharacter>(data);
        }
    }
}