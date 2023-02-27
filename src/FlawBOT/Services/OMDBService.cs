using OMDbSharp;
using OMDbSharp.Objects;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public static class OmdbService
    {
        public static async Task<Item> GetMovieDataAsync(string token, string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                query = query.ToLowerInvariant().Replace("&", "%26").Replace(" ", "+");
                return await new OMDbClient(token, false).GetItemByTitle(query).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }
    }
}