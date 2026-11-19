# Changelog

## 5.0.0

A maintenance release that brings FlawBOT up to date and fixes a large number of bugs. No new commands were added.

### Breaking changes
- **Requires the .NET 10 runtime** (was .NET 7, which is out of support).
- **Music now uses Lavalink v4.** The bundled 2019 Lavalink v3 jar was removed. Download Lavalink 4 and use the updated
  `application.yml`, which enables the YouTube plugin. See the [Lavalink guide](https://criticalflaw.ca/FlawBOT/lavalink).
- `/music` commands are only registered when Lavalink is enabled in `config.json`.
- The unused `Prefix` setting was removed from `config.json`, along with the leftover prefix-command (CommandsNext) and
  VoiceNext setup that no command used.
- Option changes: `/music play` takes `query` (a URL or search text), `/user unban` takes the user ID as text in `userid`.

### Security fixes
- Owner-only commands (`/bot avatar`, `/bot activity`, `/bot nickname`, `/bot status`) could be run by anyone, and
  `/bot leave`, `/server avatar` and `/server rename` did not check permissions. They used prefix-command attributes that
  slash commands ignore.

### Bug fixes
- The bot could not find `config.json` on Linux or macOS, and crashed waiting for a key press when run as a service.
  Ctrl+C and `SIGTERM` now shut it down cleanly.
- Music was only enabled if `Lavalink.jar` happened to be in the working directory (the build put it in `Resources`),
  and music commands replied twice or crashed when the user wasn't in a voice channel.
- Commands calling external APIs now defer their response, so slow APIs no longer cause "The application did not respond".
- Errors are reported to the user even after a command has responded, without exposing internal exception messages.
- Long text (Urban Dictionary, OMDB plot, Reddit titles) showed `System.Linq...` instead of the text.
- Embeds with a missing or overly long value (box office, release dates, categories, roles, emojis) failed the whole command.
- `/twitch` looked up the wrong streams (the channel name was passed as a pagination cursor), showed "LIVE" as the
  broadcaster and left `{width}x{height}` in the preview image link.
- `/youtube video` searched channels, and `/youtube channel` and `/youtube playlist` returned mixed result types.
- `/user unban` couldn't accept real Discord user IDs, `/user ban` sent the ban twice, `/user info` crashed without the
  presence intent and labelled the bot as the server owner.
- `/steam connect` produced a broken link and ignored the password; `/steam user` now accepts Steam IDs and profile links.
- `/tf2 search` replied twice and failed; several `/tf2` commands crashed on missing data.
- `/cat` used a shut-down API (random.cat) and now uses TheCatAPI.
- Reddit, NewsAPI and Wikipedia requests were rejected for lacking a user agent.
- `/wikipedia`, `/news` and `/reddit` reported "No results found" when fewer than five results existed.
- `/channel clean` failed on messages older than two weeks or when deleting a single message.
- `/bot nickname` changed the bot's global username instead of its server nickname.
- `/nasa` failed on days when the picture of the day is a video, or when its explanation was long.
- `/server avatar` and `/bot avatar` accepted any URL and could send an empty image.
- Shared random number generator was not thread-safe, and the shared HTTP client didn't follow redirects or time out.
- Speedrun lookups blocked on network calls and failed for games with many categories.

### Dependencies
- DSharpPlus 4.4.2 → 4.5.3, with Lavalink4NET 4.2.2 replacing DSharpPlus.Lavalink.
- SteamWebAPI2 now comes from NuGet (5.0.0) instead of a bundled fork.
- Imgur and OMDB are called directly; the abandoned Imgur.API gallery endpoints and OMDbSharp packages were removed.
- Removed unused AutoMapper, StackExchange.Redis, YoutubeExplode and Microsoft.Extensions.Options packages.
- All other packages updated to their latest stable versions.

### Tests and tooling
- New offline unit test suite (NUnit 4) covering parsing, embed building, configuration, error handling, slash-command
  definitions and startup wiring. Tests that call real APIs are in a separate `Live` category.
- The test project now builds on Linux and macOS.
- New GitHub Actions workflow builds and tests every push and pull request.
