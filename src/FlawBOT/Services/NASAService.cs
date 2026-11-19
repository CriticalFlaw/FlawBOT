using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.NASA;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class NasaService : HttpHandler
    {
        public static async Task<DiscordEmbed> GetNasaImageAsync(string token)
        {
            try
            {
                // NASA provides a rate-limited DEMO_KEY for use without registering.
                if (string.IsNullOrWhiteSpace(token)) token = "DEMO_KEY";
                var response = await Http.GetStringAsync(string.Format(Resources.URL_NASA, token)).ConfigureAwait(false);
                return BuildImageEmbed(JsonConvert.DeserializeObject<NasaData>(response));
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed BuildImageEmbed(NasaData results)
        {
            if (results is null || string.IsNullOrWhiteSpace(results.Title)) return null;
            var output = new DiscordEmbedBuilder()
                .WithTitle(results.Title.Truncate(EmbedExtensions.TitleLimit))
                .WithDescription(results.Description.Truncate(EmbedExtensions.DescriptionLimit))
                .WithColor(new DiscordColor("#0B3D91"));

            // The picture of the day is sometimes a video, which can't be shown as an embed image.
            if (results.MediaType == "image")
                output.WithImageUrl(results.ImageHd ?? results.ImageSd);
            else if (!string.IsNullOrWhiteSpace(results.ImageSd))
                output.WithUrl(results.ImageSd);

            if (!string.IsNullOrWhiteSpace(results.Copyright))
                output.WithFooter($"© {results.Copyright.Trim()}");
            return output.Build();
        }
    }
}
