using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Misc;
using FlawBOT.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Immutable;
using System.Net;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class MiscService : HttpHandler
    {
        private static readonly ImmutableArray<string> Answers = new[]
        {
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
        }.ToImmutableArray();

        public static string GetRandomAnswer()
        {
            var random = new Random();
            return Answers[random.Next(Answers.Length)];
        }

        public static async Task<DiscordEmbed> GetCatImageAsync()
        {
            var response = await Http.GetStringAsync(Resources.URL_CatPhoto).ConfigureAwait(false);
            var results = JObject.Parse(response)["file"]?.ToString();

            var responseFact = await Http.GetStringAsync(Resources.URL_CatFacts).ConfigureAwait(false);
            var resultsFact = JObject.Parse(responseFact)["fact"]?.ToString();

            var output = new DiscordEmbedBuilder()
                .WithImageUrl(results)
                .WithFooter(resultsFact)
                .WithColor(DiscordColor.Orange);
            return output.Build();
        }

        public static async Task<DiscordEmbed> GetDogImageAsync()
        {
            var response = await Http.GetStringAsync(Resources.URL_DogPhoto).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<DogData>(response);
            var results = (result.Status != "success") ? null : result;

            var output = new DiscordEmbedBuilder()
                .WithImageUrl(results.Message)
                .WithColor(DiscordColor.Brown);
            return output.Build();
        }

        public static async Task<IPLocation> GetIpLocationAsync(IPAddress query)
        {
            var result = await Http.GetStringAsync(string.Format(Resources.URL_IPAPI, query)).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IPLocation>(result);
        }
    }
}