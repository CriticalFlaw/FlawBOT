using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlawBOT.Framework.Services
{
    public static class EightBallService
    {
        private static ImmutableArray<string> Answers = new[]
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
    }

    public class CatService : HttpHandler
    {
        public static async Task<string> GetCatFactAsync()
        {
            return await _http.GetStringAsync(Resources.API_CatFacts).ConfigureAwait(false);
        }

        public static async Task<string> GetCatPhotoAsync()
        {
            var results = await _http.GetStringAsync(Resources.API_CatPhoto).ConfigureAwait(false);
            return JObject.Parse(results)["file"].ToString();
        }
    }

    public class DogService : HttpHandler
    {
        public static async Task<DogData> GetDogPhotoAsync()
        {
            var results = await _http.GetStringAsync(Resources.API_DogPhoto).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<DogData>(results);
        }
    }
}