using FlawBOT.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services.Games
{
    public class SmashService
    {
        private static readonly string base_url = "https://test-khapi.frannsoft.com/api/characters/name/";
        private static readonly HttpClient http = new HttpClient();

        public static async Task<SmashCharacter> GetSmashCharacterAsync(string query)
        {
            try
            {
                var results = await http.GetStringAsync(base_url + query + "?game=ultimate");
                return JsonConvert.DeserializeObject<SmashCharacter>(results);
            }
            catch
            {
                return null;
            }
        }
    }
}