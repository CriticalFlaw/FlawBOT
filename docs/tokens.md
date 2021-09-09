To get the most out of FlawBOT's commands, modify the included **config.json** file with below API tokens.
* [Discord][api-discord] (*required*)
* [Steam][api-steam]
* [Imgur][api-imgur]
* [OMDB][api-omdb]
* [Twitch][api-twitch]
* [NASA][api-news]
* [Teamwork.TF][api-teamwork]
* [News API][api-news]
* [WeatherStack][api-weather]
* [YouTube][api-youtube]

Below is a sample of what the final config.json will look like.
```json
{
  "Name": "FlawBOT",
  "Prefix": ".",
  "ShardCount": 1,
  "Lavalink": false,
  "Tokens": {
	  "Discord": "MzMM4kn_WRCjbDkqS5jFJD4EzMD5ODMzMDI5MD54Yz8EyNDgz.XjAxwA.k-",
	  "Steam": "CB6352D346CBA5DC46CC09SD615E4EC8",
	  "Imgur": "b1ab9562540d2c6",
	  "OMDB": "43c5439f",
	  "Twitch": "ylc3xdw6s5zqtvh9mbtemvfp2xxyb2",
	  "NASA": "QWJ2S9vVLFpO22iQaDuD4beX5VAFejO4oWFgnGUL",
	  "TeamworkTF": "Cd61htfiePyAYpIqrbaw4WgZcmBoVzHJ",
	  "News": "80ffd606b7c08fa4bf6a4e939ce01b4d",
	  "Weather": "3e75691bd1a435b1fbc3ac4cfb7a774a",
	  "YouTube": "AICghVtLUbByZzaSS9zs_nnBVrxhyxcqwAEEot0"
  }
}
```

<!-- MARKDOWN LINKS -->
[runtime-link]: https://dotnet.microsoft.com/download/dotnet/5.0/runtime
[api-discord]: https://discordapp.com/developers/applications/me
[api-steam]: https://steamcommunity.com/dev/apikey
[api-imgur]: https://api.imgur.com/oauth2/addclient
[api-omdb]: http://www.omdbapi.com/apikey.aspx
[api-twitch]: https://dev.twitch.tv/dashboard/apps/create
[api-news]: https://api.nasa.gov/
[api-teamwork]: https://github.com/teamworktf/website_api#teamworktf-json-api-for-information-about-team-fortress-2
[api-news]: https://newsapi.org/
[api-weather]: https://weatherstack.com/
[api-youtube]: https://console.cloud.google.com/projectselector/apis/credentials