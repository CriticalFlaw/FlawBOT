using System.Net.Http;

namespace FlawBOT.Services
{
    public abstract class HttpHandler
    {
        private static readonly HttpClientHandler Handler = new HttpClientHandler {AllowAutoRedirect = false};
        protected static readonly HttpClient Http = new HttpClient(Handler, true);
    }
}