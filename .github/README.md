<!-- BADGES -->
[![releases-shield]][releases-link]
[![build-shield]][build-link]
[![license-shield]][license-link]
[![docs-shield]][docs-link]

![logo](https://user-images.githubusercontent.com/6818236/206876167-497dca99-7248-4bd7-a1ae-9c0e037dc87b.png)
<p align="center">
  <p align="center">
    Multipurpose Discord bot written in C# using DSharpPlus.
    <br />
    <a href="https://github.com/CriticalFlaw/FlawBOT/releases/latest">Download</a>
    ·
    <a href="https://criticalflaw.ca/FlawBOT/">Documentation</a>
    ·
    <a href="https://github.com/DSharpPlus/DSharpPlus">DSharpPlus</a>
  </p>
</p>

<!-- MARKDOWN LINKS -->
[releases-shield]: https://img.shields.io/github/release/CriticalFlaw/FlawBOT.svg
[releases-link]: https://github.com/CriticalFlaw/FlawBOT/releases/latest
[docs-shield]: https://github.com/CriticalFlaw/FlawBOT/actions/workflows/docs.yml/badge.svg
[docs-link]: https://github.com/CriticalFlaw/FlawBOT/actions/workflows/docs.yml
[build-shield]: https://github.com/CriticalFlaw/FlawBOT/actions/workflows/build.yml/badge.svg
[build-link]: https://github.com/CriticalFlaw/FlawBOT/actions/workflows/build.yml
[license-shield]: https://img.shields.io/github/license/CriticalFlaw/FlawBOT
[license-link]: https://github.com/CriticalFlaw/FlawBOT/blob/master/LICENSE

---

### Installation
1. Install the [.NET 10 Runtime][runtime-link].
2. [Download the latest version of **flawbot.zip**][releases-link].
3. Extract downloaded file contents into a separate folder.
4. Modify the included **Resources/config.json** file with your [API tokens][tokens-link].
5. Open the command prompt and start the bot by entering: `dotnet FlawBOT.dll`
6. To use the music commands, also [set up Lavalink][lavalink-link].

### Commands
There are multiple commands available in FlawBOT. Manage your Discord server, call popular public APIs or play music in your voice channel.

[Click here for the full command documentation][commands-link].

### Building from source
Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), then run `dotnet build src/FlawBOT.sln` and `dotnet test src/FlawBOT.sln`.
See the [deployment guide](https://criticalflaw.ca/FlawBOT/deploy) for details.

### API Tokens
To get the most out of FlawBOT's commands, modify the included **config.json** file with the required API tokens.

[Click here for the full list of API tokens and where to find them.][tokens-link]

<!-- MARKDOWN LINKS -->
[releases-link]: https://github.com/CriticalFlaw/FlawBOT/releases/latest
[tokens-link]: https://criticalflaw.ca/FlawBOT/tokens/
[lavalink-link]: https://criticalflaw.ca/FlawBOT/lavalink/
[runtime-link]: https://dotnet.microsoft.com/download/dotnet/10.0
[commands-link]: https://criticalflaw.ca/FlawBOT/modules/bot
