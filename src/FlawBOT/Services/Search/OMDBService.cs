using System.Threading.Tasks;
using OMDbSharp;
using OMDbSharp.Objects;

namespace FlawBOT.Services
{
    public static class OmdbService
    {
        public static async Task<Item> GetMovieDataAsync(string token, string query)
        {
            return await new OMDbClient(token, false)
                .GetItemByTitle(query.ToLowerInvariant().Replace("&", "%26")).ConfigureAwait(false);
        }

        public static async Task<ItemList> GetMovieListAsync(string token, string query)
        {
            return await new OMDbClient(token, false)
                .GetItemList(query.ToLowerInvariant().Replace("&", "%26")).ConfigureAwait(false);
        }
    }
}