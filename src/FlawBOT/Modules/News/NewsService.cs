using FlawBOT.Common;
using FlawBOT.Models.News;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class NewsService : HttpHandler
    {
        public static async Task<NewsData> GetNewsDataAsync(string token, string query = "")
        {
            var results = await Http
                .GetStringAsync(string.Format(Resources.URL_News, query.ToLowerInvariant(), token))
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<NewsData>(results);
        }
    }
}