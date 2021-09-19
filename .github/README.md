<!-- BADGES -->
[![version-shield]][releases-link]
[![releases-shield]][releases-link]
[![docs-shield]][docs-link]
[![license-shield]][license-link]
[![issues-shield]][issues-link]
[![invite-shield]][invite-link]

![image](https://user-images.githubusercontent.com/6818236/133937428-8f74b640-52f9-4291-bf42-62929c9938a0.png)
<p align="center">
  <p align="center">
    Discord bot written in C# using DSharpPlus. Manage your server, play music and call popular APIs.
    <br />
    <a href="https://github.com/CriticalFlaw/FlawBOT/releases/latest">Download</a>
    ·
    <a href="https://www.flawbot.criticalflaw.ca/">Documentation</a>
    ·
    <a href="https://github.com/DSharpPlus/DSharpPlus">DSharpPlus</a>
    ·
    <a href="https://github.com/CriticalFlaw/TF2HUD.Editor/issues">Issue Tracker</a>
    ·
    <a href="https://discordapp.com/oauth2/authorize?client_id=339833029013012483&scope=bot&permissions=66186303">Invite FlawBOT</a>
  </p>
</p>

<!-- MARKDOWN LINKS -->
[version-shield]: https://img.shields.io/github/release/CriticalFlaw/FlawBOT.svg?style=flat-square
[releases-shield]: https://img.shields.io/github/downloads/criticalflaw/flawbot/total?style=flat-square
[releases-link]: https://github.com/CriticalFlaw/FlawBOT/releases/latest
[docs-shield]: https://readthedocs.org/projects/flawbot/badge/?version=latest&style=flat-square
[docs-link]: https://www.flawbot.criticalflaw.ca/
[license-shield]: https://img.shields.io/github/license/CriticalFlaw/FlawBOT?style=flat-square
[license-link]: https://github.com/CriticalFlaw/FlawBOT/blob/master/.github/LICENSE
[issues-shield]: https://img.shields.io/github/issues/CriticalFlaw/FlawBOT?style=flat-square
[issues-link]: https://github.com/CriticalFlaw/FlawBOT/issues
[invite-shield]: https://img.shields.io/badge/Discord-invite-7289da.svg?style=flat-square&logo=discord
[invite-link]: https://discordapp.com/oauth2/authorize?client_id=339833029013012483&scope=bot&permissions=66186303
[tokens-link]: https://www.flawbot.criticalflaw.ca/tokens/
[runtime-link]: https://dotnet.microsoft.com/download/dotnet/5.0/runtime

---
                                                           
### Basic Installation
1. [Download the latest version of **flawbot.zip**][releases-link].
2. Extract downloaded file contents into a separate folder.
3. Modify the included **config.json** file with your [API tokens](#API-Tokens).
4. Open the command prompt and start the bot by entering: `dotnet FlawBOT.dll`
   * In case of errors, install the [.NET Core 5.0 Runtime for **console apps**][runtime-link].

### Advanced Installation
1. Clone the repository.
2. Open `FlawBOT\src\FlawBOT.sln` in [Visual Studio 2019][vs-link].
3. Modify the included **config.json** file with your [API tokens](#API-Tokens).
4. To use music commands, run the included `Lavalink.jar` file.
5. Build and run the project.

---

### Command Modules
Following is a list of command modules included with FlawBOT. Check the documentation for details.
<p align="east">
    <a href="https://www.flawbot.criticalflaw.ca/cmd/bot/">Bot</a>
    ·
    <a href="https://www.flawbot.criticalflaw.ca/cmd/games/">Games</a>
    ·
    <a href="https://www.flawbot.criticalflaw.ca/cmd/role/music/">Music</a>
    ·
    <a href="https://www.flawbot.criticalflaw.ca/cmd/search/">Search</a>
    ·
    <a href="https://www.flawbot.criticalflaw.ca/cmd/role/misc/">Misc</a>
    ·
    <a href="https://www.flawbot.criticalflaw.ca/cmd/server/">Server</a>
    ·
    <a href=https://www.flawbot.criticalflaw.ca/cmd/channel/">Channel</a>
    ·
    <a href="https://www.flawbot.criticalflaw.ca/cmd/user/">User</a>
    ·
    <a href="https://www.flawbot.criticalflaw.ca/cmd/role/">Role</a>
    ·
    <a href="https://www.flawbot.criticalflaw.ca/cmd/emoji/">Emoji</a>
  </p>
</p>

### API-Tokens
Below are links to sites where you can generate your API tokens to then put into the included **config.json** file.
<p align="east">
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
    <a href="https://github.com/teamworktf/website_api">Teamwork.TF</a>
    ·
    <a href="https://newsapi.org/">NewsAPI</a>
    ·
    <a href="https://weatherstack.com/">WeatherStack</a>
    ·
    <a href="https://console.cloud.google.com/projectselector/apis/credentials">YouTube</a>
  </p>
</p>

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
[vs-link]: https://visualstudio.microsoft.com/
[lava-link]: https://github.com/Frederikam/Lavalink#requirements
[java-link]: https://www.oracle.com/java/technologies/javase-jdk13-downloads.html

