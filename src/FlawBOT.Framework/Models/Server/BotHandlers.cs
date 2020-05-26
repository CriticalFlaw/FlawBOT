using System.Net.Http;

namespace FlawBOT.Framework.Models
{
    public abstract class HttpHandler
    {
        protected static readonly HttpClientHandler Handler = new HttpClientHandler {AllowAutoRedirect = false};
        protected static readonly HttpClient Http = new HttpClient(Handler, true);
    }

    public class TokenHandler
    {
        public static TokenData Tokens { get; set; } = new TokenData();
    }
}