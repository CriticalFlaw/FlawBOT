---
title: Tokens
---

To get the most out of FlawBOT's commands, modify the included **config.json** file with the API tokens below.
Only the Discord token is required. Commands whose token is missing reply with "No results found."

| Token | Used by | Notes |
|---|---|---|
| [Discord][discord-link] | Everything | **Required.** The bot token from the Bot page of your application. |
| [Imgur][imgur-link] | `/imgur` | The application's Client ID. |
| [NASA][nasa-link] | `/nasa` | Optional. Uses NASA's rate-limited `DEMO_KEY` when empty. |
| [NewsAPI][news-link] | `/news` | |
| [OMDB][omdb-link] | `/omdb` | |
| [Pokemon][pokemon-link] | `/pokemon` | Optional. Raises the Pokémon TCG API rate limit. |
| [Steam][steam-link] | `/steam`, `/tf2 item`, `/tf2 creator` | `/steam game` also works without a token. |
| [Teamwork.TF][teamwork-link] | `/tf2` | |
| [Twitch][twitch-link] | `/twitch` | `Twitch` is the Client ID and `TwitchAccess` is an app access token. |
| [Weather][weather-link] | `/weather` | Weatherstack access key. |
| [YouTube][youtube-link] | `/youtube` | YouTube Data API v3 key. |

```json
{
	"Name": "FlawBOT",
	"ShardCount": 1,
	"Lavalink": {
		"Enabled": false,
		"Address": "127.0.0.1",
		"Port": 2333,
		"Password": "youshallnotpass"
	},
	"Tokens": {
		"Discord": "MzM5OD123435EyNDgz.Gh3bSD.ryJQ1--BJDn_4sWtXSX345SdFUbb49Y",
		"Steam": "CB63523458CC09AB615E4EC8",
		"Imgur": "b1ab10234d2c6",
		"Pokemon": "e8494d53-a6f9-4c8b-234-e75f8b84b37c",
		"OMDB": "76c8889f",
		"Twitch": "ylc3xvh9mbte456s5zqtxyb2",
		"TwitchAccess": "ylc3xvh9mbtemvf6546s5zqtxyb2",
		"NASA": "4al2olXC60FxVhBpA4563bkEml8XKtl8e",
		"TeamworkTF": "CAYpI456ZcmBd61htfiePyoVzHJ",
		"News": "b7c080f4bf6a4e939c456e01b4d",
		"Weather": "3fb7a7791bd456b14ae756fbc3ac4c",
		"YouTube": "AIzaSyCghVt456_nnBVrxhEEot0"
	}
}
```

:::note
Twitch app access tokens expire after about 60 days. Generate a new one with the
[client credentials flow][twitch-token-link] when `/twitch` stops returning results.
:::

<!-- MARKDOWN LINKS -->
[discord-link]: https://discord.com/developers/applications
[steam-link]: https://steamcommunity.com/dev/apikey
[imgur-link]: https://api.imgur.com/oauth2/addclient
[omdb-link]: https://www.omdbapi.com/apikey.aspx
[pokemon-link]: https://dev.pokemontcg.io/
[twitch-link]: https://dev.twitch.tv/console/apps/create
[twitch-token-link]: https://dev.twitch.tv/docs/authentication/getting-tokens-oauth/#client-credentials-grant-flow
[nasa-link]: https://api.nasa.gov/
[teamwork-link]: https://github.com/teamworktf/website_api#teamworktf-json-api-for-information-about-team-fortress-2
[news-link]: https://newsapi.org/
[weather-link]: https://weatherstack.com/
[youtube-link]: https://console.cloud.google.com/apis/credentials
