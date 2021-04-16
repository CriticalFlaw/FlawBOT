[![banner][banner-image]]
[![version][version-badge]]
[![build][build-badge][build-link]]
[![quality][quality-badge][quality-link]]

A Discord bot written in C# using the [DSharpPlus][dsharp-link] library. This bot is not being hosted online, however you're welcome to fork this repository and host it yourself or reuse the code for your own bot, as long as you credit it properly. 

## Invite
[Click here to invite FlawBOT to your server][invite-link]. Please note that the bot will only be online periodically for testing purposes.

## Installation
To run your own instance of FlawBOT, [clone this repository][clone-link] or [download the release package][release-link] and modify the provided **config.json** file, adding the API tokens. If you're cloning the repository, use [Visual Studio 2019][vs-link] to compile the project. 

## Running FlawBOT
As of version 3.0, FlawBOT requires a [Lavalink][lava-link] node to be running in order to play music and not display lavalink-related errors in console. `lavalink.jar` and `application.yml` are included in the project directory, so all you have to do is open the command prompt and launch lavalink with the command `java -jar Lavalink.jar` (be sure to install [Java 13 or later][java-link].

## Documentation
For more information on the project, including the complete list of commands can be found [here][docs-link].

## API Tokens
* [Discord][api-discord] (*required*)
* [Steam](
* [Imgur][api-imgur]
* [OMDB][api-omdb]
* [Twitch][api-twitch]
* [NASA][api-news]
* [Teamwork.TF][api-teamwork]
* [News API][api-news]
* [WeatherStack][api-weather]
* [YouTube][api-youtube]


<!-- MARKDOWN LINKS -->
[banner-image]: https://i.imgur.com/YlbST5I.jpg
[version-badge]: https://img.shields.io/github/release/CriticalFlaw/FlawBOT.svg
[build-badge]: https://ci.appveyor.com/api/projects/status/6hw48u0v6muwxvvo?svg=true
[build-link]: https://ci.appveyor.com/project/CriticalFlaw/flawbot
[quality-badge]: https://api.codacy.com/project/badge/Grade/1747be5dd35645369b747b81cc86701c
[quality-link]: https://www.codacy.com/app/CriticalFlaw/FlawBOT?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=CriticalFlaw/FlawBOT&amp;utm_campaign=Badge_Grade
[dsharp-link]: https://github.com/DSharpPlus/DSharpPlus
[docs-link]: https://flawbot.criticalflaw.ca/
[invite-link]: https://discordapp.com/oauth2/authorize?client_id=339833029013012483&scope=bot&permissions=66186303
[clone-link]: https://github.com/CriticalFlaw/FlawBOT/archive/master.zip
[release-link]: https://github.com/CriticalFlaw/FlawBOT/releases
[vs-link]: https://visualstudio.microsoft.com/
[lava-link]: https://github.com/Frederikam/Lavalink#requirements
[java-link]: https://www.oracle.com/java/technologies/javase-jdk13-downloads.html
[api-discord]: https://discordapp.com/developers/applications/me
[api-steam]: https://steamcommunity.com/dev/apikey
[api-imgur]: https://api.imgur.com/oauth2/addclient
[api-omdb]: http://www.omdbapi.com/apikey.aspx
[api-twitch]: https://dev.twitch.tv/dashboard/apps/create
[api-news]: https://api.nasa.gov/
[api-teamwork]: https://github.com/teamworktf/website_api
[api-news]: https://newsapi.org/
[api-weather]: https://weatherstack.com/
[api-youtube]: https://console.cloud.google.com/projectselector/apis/credentials