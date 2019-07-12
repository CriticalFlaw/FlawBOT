using FlawBOT.Framework.Common;
using Reddit;
using Reddit.Controllers;

namespace FlawBOT.Framework.Services
{
    public class RedditService
    {
        public static Subreddit GetSubredditAsync(string query)
        {
            var client = new RedditAPI(appId: TokenHandler.Tokens.RedditAppToken, refreshToken: TokenHandler.Tokens.RedditAccessToken, accessToken: TokenHandler.Tokens.RedditRefreshToken);
            var results = client.Subreddit(query).About();
            return results;
        }
    }
}