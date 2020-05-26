using FlawBOT.Framework.Models;
using Google.Apis.Books.v1;
using Google.Apis.Services;

namespace FlawBOT.Framework.Services
{
    public class BookService : HttpHandler
    {
        public BookService()
        {
            Books = new BooksService(new BaseClientService.Initializer
            {
                ApiKey = TokenHandler.Tokens.GoogleToken,
                ApplicationName = "FlawBOT"
            });
        }

        private BooksService Books { get; }
    }
}