using System.Threading.Tasks;
using FlawBOT.Common;
using FlawBOT.Models;
using FlawBOT.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Services
{
    public class NewsService : HttpHandler
    {
        public static async Task<NewsData> GetNewsDataAsync(string query = "")
        {
            var results = await Http
                .GetStringAsync(string.Format(Resources.URL_News, query, SharedData.Tokens.NewsToken))
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<NewsData>(results);
        }
    }
}