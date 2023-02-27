using FlawBOT.Common;
using FlawBOT.Models.News;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace FlawBOT.Services
{
    public class NewsService : HttpHandler
    {
        public static async Task<List<Article>> GetNewsDataAsync(string token, string query)
        {
            try
            {
                query = string.Format(Resources.URL_News, query.ToLowerInvariant(), token);
                var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<NewsData>(response);
                if (result.Status != "ok" || result.Articles.Count < 5) return null;
                return result.Articles.OrderBy(x => random.Next()).Take(5).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}