using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.News;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class NewsService : HttpHandler
    {
        public static async Task<DiscordEmbed> GetNewsArticlesAsync(string token, string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(query)) return null;
                var url = string.Format(Resources.URL_News, Uri.EscapeDataString(query.Trim()), token);
                var response = await Http.GetStringAsync(url).ConfigureAwait(false);
                return BuildArticlesEmbed(JsonConvert.DeserializeObject<NewsData>(response));
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed BuildArticlesEmbed(NewsData result)
        {
            if (result?.Status != "ok" || result.Articles is null) return null;
            var results = result.Articles
                .Where(x => !string.IsNullOrWhiteSpace(x.Title) && !string.IsNullOrWhiteSpace(x.Url) && x.Title != "[Removed]")
                .OrderBy(_ => Random.Next()).Take(5).ToList();
            if (results.Count == 0) return null;

            var output = new DiscordEmbedBuilder()
                .WithTitle("Latest news articles from NewsAPI.")
                .WithColor(new DiscordColor("#253B80"));
            foreach (var x in results)
                output.AddSafeField(x.PublishDate.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), $"[{x.Title.Truncate(300)}]({x.Url})");
            return output.Build();
        }
    }
}
