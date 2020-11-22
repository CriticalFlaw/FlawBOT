![img](https://i.imgur.com/YlbST5I.jpg)
![Version Badge](https://img.shields.io/github/release/CriticalFlaw/FlawBOT.svg)
[![Appveyor Status](https://ci.appveyor.com/api/projects/status/6hw48u0v6muwxvvo?svg=true)](https://ci.appveyor.com/project/CriticalFlaw/flawbot)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/1747be5dd35645369b747b81cc86701c)](https://www.codacy.com/app/CriticalFlaw/FlawBOT?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=CriticalFlaw/FlawBOT&amp;utm_campaign=Badge_Grade)

A Discord bot written in C# using the [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus) library. This bot is not being hosted online, however you're welcome to fork this repository and host it yourself or reuse the code for your own bot, as long as you credit it properly. The complete list of commands and other documentation can be found [here](https://github.com/CriticalFlaw/FlawBOT/wiki). 

## Invite
[Click here to invite FlawBOT to your server](https://discordapp.com/oauth2/authorize?client_id=339833029013012483&scope=bot&permissions=66186303). Please note that the bot will only be online periodically for testing purposes.

## Installation
To run your own instance of FlawBOT, [clone this repository](https://github.com/CriticalFlaw/FlawBOT/archive/master.zip) or [download the release package](https://github.com/CriticalFlaw/FlawBOT/releases) and modify the provided **config.json** file, adding the API tokens. If you're cloning the repository, use [Visual Studio 2019](https://www.visualstudio.com/downloads/) to compile the project. 

## Running FlawBOT
As of version 3.0, FlawBOT requires a [Lavalink](https://github.com/Frederikam/Lavalink#requirements) node to be running in order to play music and not display lavalink-related errors in console. `lavalink.jar` and `application.yml` are included in the project directory, so all you have to do is open the command prompt and launch lavalink with the command `java -jar Lavalink.jar` (be sure to install [Java 13 or later](https://www.oracle.com/java/technologies/javase-jdk13-downloads.html)).

## API Tokens
* [Discord](https://discordapp.com/developers/applications/me) (*required*)
* [Steam](https://steamcommunity.com/dev/apikey)
* [Imgur](https://api.imgur.com/oauth2/addclient)
* [OMDB](http://www.omdbapi.com/apikey.aspx)
* [Twitch](https://dev.twitch.tv/dashboard/apps/create)
* [NASA](https://api.nasa.gov/)
* [Teamwork.TF](https://github.com/teamworktf/website_api)
* [News API](https://newsapi.org/)
* [WeatherStack](https://weatherstack.com/)
* [YouTube](https://console.cloud.google.com/projectselector/apis/credentials)