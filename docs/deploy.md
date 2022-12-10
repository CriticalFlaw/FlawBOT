---
title: How to Deploy FlawBOT
---

1. [Clone][clone-link] or [Download][download-link] the source code.
2. Open `FlawBOT\src\FlawBOT.sln` in [Visual Studio][vs-link].
3. Modify the included **config.json** file with your [API tokens][tokens-link].
4. Build the project.
```
dotnet clean -c Release
dotnet restore
dotnet build -c Release -f net7.0
dotnet publish -c Release -f net7.0
```
5. Update your Raspberry Pi to the latest version.
```
sudo apt-get update -y
sudo apt-get upgrade -y
```
6. Copy the compiled FlawBOT files to the Raspberry Pi.
7. Install .NET Core 7.0 runtime.
```
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel Current
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc
```
8. Navigate to where you've copied your FlawBOT files and start the bot.
```
dotnet FlawBOT.dll
```

<!-- MARKDOWN LINKS -->
[clone-link]: https://github.com/CriticalFlaw/FlawBOT.git
[download-link]: https://github.com/CriticalFlaw/FlawBOT/archive/refs/heads/master.zip
[vs-link]: https://visualstudio.microsoft.com/
[tokens-link]: https://www.criticalflaw.ca/FlawBOT/tokens