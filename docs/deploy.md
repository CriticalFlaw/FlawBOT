---
title: How to Deploy FlawBOT
---

These steps build FlawBOT from source and run it on a Linux machine such as a Raspberry Pi (64-bit OS). They work the same on any other machine with the .NET runtime.

1. [Download][download-link] the latest FlawBOT source code.
2. Install the [.NET 10 SDK][sdk-link] on the machine you'll build on.
3. Modify the included `src/FlawBOT/Resources/config.json` file with your [API tokens][tokens-link].
4. Build and publish the bot.
```
dotnet publish src/FlawBOT/FlawBOT.csproj -c Release -o publish
```
5. Update your Raspberry Pi to the latest version.
```
sudo apt-get update -y
sudo apt-get upgrade -y
```
6. Copy the contents of the `publish` folder to the Raspberry Pi.
7. Install the .NET 10 runtime on the Raspberry Pi.
```
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0 --runtime dotnet
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc
```
8. Navigate to where you've copied your FlawBOT files and start the bot.
```
dotnet FlawBOT.dll
```

FlawBOT shuts down cleanly when it receives Ctrl+C or `SIGTERM`, so it can be run as a `systemd` service or in a container.

### Discord application settings
In the [Discord Developer Portal][discord-portal], invite the bot with the `bot` and `applications.commands` scopes.
FlawBOT uses slash commands only, so the **Message Content Intent** is not needed. Enable the **Server Members Intent**
if you want to use `/role inrole`.

### Running the tests
```
dotnet test src/FlawBOT.sln
```
This runs the offline unit tests. Tests that call the real APIs are in the `Live` category and are skipped by default.
To run them, add your tokens to `config.json` or set `FLAWBOT_TOKEN_<NAME>` environment variables (for example `FLAWBOT_TOKEN_NASA`), then run:
```
dotnet test src/FlawBOT.sln -- NUnit.Where="cat == Live"
```
Live tests for APIs without a configured token are skipped.

<!-- MARKDOWN LINKS -->
[download-link]: https://github.com/CriticalFlaw/FlawBOT/releases/latest
[sdk-link]: https://dotnet.microsoft.com/download/dotnet/10.0
[discord-portal]: https://discord.com/developers/applications
[tokens-link]: https://criticalflaw.ca/FlawBOT/tokens
