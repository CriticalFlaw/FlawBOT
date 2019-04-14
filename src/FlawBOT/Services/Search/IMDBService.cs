using FlawBOT.Common;
using OMDbSharp;
using System.Threading.Tasks;

namespace FlawBOT.Services.Search
{
    public class IMDBService
    {
        public static async Task<OMDbSharp.Objects.Item> GetMovieDataAsync(string query)
        {
            var client = new OMDbClient(SharedData.Tokens.OMDBToken, true);
            return await client.GetItemByTitle(query.Replace("&", "%26"));
        }
    }
}