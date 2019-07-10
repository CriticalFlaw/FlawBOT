using FlawBOT.Common;
using Reddit;
using Reddit.Controllers;

namespace FlawBOT.Services.Search
{
    public class RedditService
    {
        public static Subreddit GetSubredditAsync(string query)
        {
            var client = new RedditAPI(appId: SharedData.Tokens.RedditAppToken, refreshToken: SharedData.Tokens.RedditAccessToken, accessToken: SharedData.Tokens.RedditRefreshToken);
            var results = client.Subreddit(query).About();
            return results;
        }
    }
}