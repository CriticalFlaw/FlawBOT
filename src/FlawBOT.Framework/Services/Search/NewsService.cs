using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Services
{
    public class NewsService : HttpHandler
    {
        public static async Task<NewsData> GetNewsDataAsync(string query = "")
        {
            var results = await Http
                .GetStringAsync(string.Format(Resources.API_News, query, TokenHandler.Tokens.NewsToken))
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<NewsData>(results);
        }
    }
}