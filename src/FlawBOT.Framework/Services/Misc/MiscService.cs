using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class EightBallService
    {
        public static string GetAnswer()
        {
            var random = new Random();
            return Answers.ElementAt(random.Next(Answers.Count()));
        }

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
    }

    public class CatService : HttpHandler
    {
        public static async Task<string> GetCatFactAsync()
        {
            return await _http.GetStringAsync("https://catfact.ninja/fact");
        }

        public static async Task<string> GetCatPhotoAsync()
        {
            var results = await _http.GetStringAsync("http://aws.random.cat/meow").ConfigureAwait(false);
            return JObject.Parse(results)["file"].ToString();
        }
    }

    public class DogService : HttpHandler
    {
        public static async Task<DogData> GetDogPhotoAsync()
        {
            var results = await _http.GetStringAsync("https://dog.ceo/api/breeds/image/random");
            return JsonConvert.DeserializeObject<DogData>(results);
        }
    }
}