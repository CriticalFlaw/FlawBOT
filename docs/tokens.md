To get the most out of FlawBOT's commands, modify the included **config.json** file with below API tokens.
<p align="center">
  <p align="center">
    <a href="https://discordapp.com/developers/applications/me">Discord</a>
    ·
    <a href="https://steamcommunity.com/dev/apikey">Steam</a>
    ·
    <a href="https://api.imgur.com/oauth2/addclient">Imgur</a>
    ·
    <a href="http://www.omdbapi.com/apikey.aspx">OMDB</a>
    ·
    <a href="https://dev.twitch.tv/dashboard/apps/create">Twitch</a>
    ·
    <a href="https://api.nasa.gov/">NASA</a>
    ·
    <a href="https://github.com/teamworktf/website_api#teamworktf-json-api-for-information-about-team-fortress-2">Teamwork.TF</a>
    ·
    <a href="https://newsapi.org/">NewsAPI</a>
    ·
    <a href="https://weatherstack.com/">WeatherStack</a>
    ·
    <a href="https://console.cloud.google.com/projectselector/apis/credentials">YouTube</a>
  </p>
 </p>
 
Below is a sample of what the final config.json will look like.
```json
{
  "Name": "FlawBOT",
  "Prefix": ".",
  "ShardCount": 1,
  "Lavalink": {
    "Enabled": true,
    "Address": "192.168.2.38",
    "Port": 2333,
    "Password": "youshallnotpass"
  },
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