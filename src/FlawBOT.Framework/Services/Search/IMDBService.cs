using FlawBOT.Framework.Common;
using OMDbSharp;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class OMDBService
    {
        public static async Task<OMDbSharp.Objects.Item> GetMovieDataAsync(string query)
        {
            return await new OMDbClient(TokenHandler.Tokens.OMDBToken, true).GetItemByTitle(query.Replace("&", "%26"));
        }
    }
}