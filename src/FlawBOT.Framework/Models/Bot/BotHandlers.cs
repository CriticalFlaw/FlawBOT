using System.Net.Http;

namespace FlawBOT.Framework.Models
{
    public abstract class HttpHandler
    {
        protected static readonly HttpClient _http = new HttpClient(_handler, true);
        protected static readonly HttpClientHandler _handler = new HttpClientHandler { AllowAutoRedirect = false };
    }

    public class TokenHandler
    {
        public static TokenData Tokens { get; set; } = new TokenData();
    }
}