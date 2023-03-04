using FlawBOT.Common;
using FlawBOT.Models.News;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using DSharpPlus.Entities;
using System.Globalization;

namespace FlawBOT.Services
{
    public class NewsService : HttpHandler
    {
        public static async Task<DiscordEmbed> GetNewsDataAsync(string token, string query)
        {
            try
            {
                query = string.Format(Resources.URL_News, query.ToLowerInvariant(), token);
                var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<NewsData>(response);
                if (result.Status != "ok" || result.Articles.Count < 5) return null;
                var results = result.Articles.OrderBy(x => random.Next()).Take(5).ToList();

                // TODO: Add pagination when supported for slash commands.
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Latest Google News articles from News API.")
                    .WithColor(new DiscordColor("#253B80"));

                foreach (var x in results)
                    output.AddField(x.PublishDate.ToString(CultureInfo.InvariantCulture), $"[{x.Title}]({x.Url})");
                return output.Build();
            }
            catch
            {
                return null;
            }
        }
    }
}