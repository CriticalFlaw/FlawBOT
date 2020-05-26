using System.Net.Http;

namespace FlawBOT.Framework.Models
{
    public abstract class HttpHandler
    {
        protected static readonly HttpClientHandler _handler = new HttpClientHandler {AllowAutoRedirect = false};
        protected static readonly HttpClient _http = new HttpClient(_handler, true);
    }

    public class TokenHandler
    {
        public static TokenData Tokens { get; set; } = new TokenData();
    }
}