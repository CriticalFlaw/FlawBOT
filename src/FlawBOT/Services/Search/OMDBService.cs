using System.Threading.Tasks;
using FlawBOT.Common;
using OMDbSharp;
using OMDbSharp.Objects;

namespace FlawBOT.Services
{
    public static class OmdbService
    {
        public static async Task<Item> GetMovieDataAsync(string query)
        {
            return await new OMDbClient(Program.Settings.Tokens.OmdbToken, false)
                .GetItemByTitle(query.ToLowerInvariant().Replace("&", "%26")).ConfigureAwait(false);
        }

        public static async Task<ItemList> GetMovieListAsync(string query)
        {
            return await new OMDbClient(Program.Settings.Tokens.OmdbToken, false)
                .GetItemList(query.ToLowerInvariant().Replace("&", "%26")).ConfigureAwait(false);
        }
    }
}