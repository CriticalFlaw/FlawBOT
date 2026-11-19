using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Misc;
using FlawBOT.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class MiscService : HttpHandler
    {
        public static readonly ImmutableArray<string> Answers =
        [
            "It is certain",
            "It is decidedly so",
            "Without a doubt",
            "Yes definitely",
            "You may rely on it",
            "As I see it, yes",
            "Most likely",
            "Outlook good",
            "Yes",
            "Signs point to yes",
            "Reply hazy try again",
            "Ask again later",
            "Better not tell you now",
            "Cannot predict now",
            "Concentrate and ask again",
            "Don't count on it",
            "My reply is no",
            "My sources say no",
            "Outlook not so good",
            "Very doubtful"
        ];

        public static string GetRandomAnswer()
        {
            return Answers[Random.Next(Answers.Length)];
        }

        public static async Task<DiscordEmbed> GetCatImageAsync()
        {
            try
            {
                // The previous provider (random.cat) has shut down; TheCatAPI's search endpoint needs no key.
                var response = await Http.GetStringAsync(Resources.URL_CatPhoto).ConfigureAwait(false);
                var image = JArray.Parse(response).First?["url"]?.ToString();
                if (string.IsNullOrWhiteSpace(image)) return null;

                var output = new DiscordEmbedBuilder()
                    .WithImageUrl(image)
                    .WithColor(DiscordColor.Orange);

                // The fact is a nice-to-have; still show the picture if the facts API is down.
                try
                {
                    var responseFact = await Http.GetStringAsync(Resources.URL_CatFacts).ConfigureAwait(false);
                    var fact = JObject.Parse(responseFact)["fact"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(fact)) output.WithFooter(fact.Truncate(EmbedExtensions.FooterLimit));
                }
                catch
                {
                    // Ignore
                }

                return output.Build();
            }
            catch
            {
                return null;
            }
        }

        public static async Task<DiscordEmbed> GetDogImageAsync()
        {
            try
            {
                var response = await Http.GetStringAsync(Resources.URL_DogPhoto).ConfigureAwait(false);
                return BuildDogEmbed(JsonConvert.DeserializeObject<DogData>(response));
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed BuildDogEmbed(DogData result)
        {
            if (result?.Status != "success" || string.IsNullOrWhiteSpace(result.Message)) return null;
            return new DiscordEmbedBuilder()
                .WithImageUrl(result.Message)
                .WithColor(DiscordColor.Brown)
                .Build();
        }
    }
}
