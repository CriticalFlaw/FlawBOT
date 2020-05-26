using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using OMDbSharp;
using OMDbSharp.Objects;

namespace FlawBOT.Framework.Services
{
    public static class OMDBService
    {
        public static async Task<Item> GetMovieDataAsync(string query)
        {
            return await new OMDbClient(TokenHandler.Tokens.OMDBToken, false)
                .GetItemByTitle(query.ToLowerInvariant().Replace("&", "%26")).ConfigureAwait(false);
        }
    }
}