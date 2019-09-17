using System.Threading.Tasks;
using FlawBOT.Framework.Common;
using Goodreads;
using Goodreads.Models.Response;

namespace FlawBOT.Framework.Services.Search
{
    public class GoodReadService : HttpHandler
    {
        public static async Task<Book> GetBookDataAsync(string query)
        {
            var client = GoodreadsClient.Create(TokenHandler.Tokens.GoodReadsKey, TokenHandler.Tokens.GoodReadsSecret);
            var results = await client.Books.GetByTitle(query);
            return results;
        }
    }
}