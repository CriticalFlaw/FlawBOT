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
            var response = await Http.GetStringAsync(string.Format(Resources.URL_NASA, token)).ConfigureAwait(false);
            var results = JsonConvert.DeserializeObject<NasaData>(response);

            var output = new DiscordEmbedBuilder()
                .WithDescription(results.Title)
                .WithImageUrl(results.ImageHd ?? results.ImageSd)
                .WithFooter(results.Description)
                .WithColor(new DiscordColor("#0B3D91"));
            return output.Build();
        }
    }
}