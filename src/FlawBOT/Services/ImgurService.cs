using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Imgur;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class ImgurService : HttpHandler
    {
        /// <summary>
        ///     Searches the Imgur gallery and returns a random safe-for-work image from the results.
        /// </summary>
        public static async Task<DiscordEmbed> GetImgurResultAsync(string token, string query)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(query)) return null;
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, string.Format(Resources.URL_Imgur_Search, Uri.EscapeDataString(query.Trim())));
                request.Headers.Authorization = new AuthenticationHeaderValue("Client-ID", token);
                using var response = await Http.SendAsync(request).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return BuildGalleryEmbed(JsonConvert.DeserializeObject<ImgurGalleryResponse>(json));
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed BuildGalleryEmbed(ImgurGalleryResponse result)
        {
            var items = result?.Data?.Where(x => !x.Nsfw && !string.IsNullOrWhiteSpace(x.GetImageLink())).ToList();
            if (items is null || items.Count == 0) return null;

            var item = items[Random.Next(items.Count)];
            return new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#89C623"))
                .WithImageUrl(item.GetImageLink())
                .WithUrl(item.Link)
                .WithFooter((item.Title ?? "Found via Imgur").Truncate(EmbedExtensions.FooterLimit))
                .Build();
        }
    }
}
