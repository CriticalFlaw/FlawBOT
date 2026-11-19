using System;
using System.Net.Http;

namespace FlawBOT.Common
{
    public abstract class HttpHandler
    {
        /// <summary>
        ///     User agent sent with every outgoing request. Several APIs (Reddit, NewsAPI, Wikipedia) reject requests without one.
        /// </summary>
        public const string UserAgent = "FlawBOT/5.0 (+https://github.com/CriticalFlaw/FlawBOT)";

        /// <summary>
        ///     Shared HTTP client for all services. Keep a single instance to avoid socket exhaustion.
        /// </summary>
        protected static readonly HttpClient Http = CreateClient();

        /// <summary>
        ///     Thread-safe random number generator shared by all services.
        /// </summary>
        protected static Random Random => Random.Shared;

        private static HttpClient CreateClient()
        {
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = true,
                PooledConnectionLifetime = TimeSpan.FromMinutes(15)
            };
            var client = new HttpClient(handler, true)
            {
                // Slash command interactions expire after 15 minutes, but users won't wait that long.
                Timeout = TimeSpan.FromSeconds(20)
            };
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            return client;
        }
    }
}
