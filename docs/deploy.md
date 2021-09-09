This guide assumes that you've already cloned, configured, compiled and ran an instance of FlawBOT on your local machine.

1. Compile FlawBOT from Visual Studio on your local machine.
```
dotnet clean -c Release
dotnet restore
dotnet build -c Release -f net5.0
dotnet publish -c Release -f net5.0
```
2. Copy the compiled FlawBOT files to your Raspberry Pi.
3. Update the Pi to the latest version.
```
sudo apt-get update -y
sudo apt-get upgrade -y
```
4. Install .NET Core 5.0 runtime.
```
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel Current
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc
```
5. Navigate to where you copied your FlawBOT files and start the bot.
```
dotnet FlawBOT.dll
```
