## Raspberry Pi Deployment

Installing .NET Core 2.0 Runtime on the Raspberry Pi

1. Login to your Raspberry Pi
2. Go into the tmp folder: `cd /tmp`
3. Download the .NET Core 2.0 ARM Runtime: `curl -LO https://dotnetcli.blob.core.windows.net/dotnet/Runtime/release/2.0.0/dotnet-runtime-latest-linux-arm.tar.gz`
4. Create a directory called dotnet in /opt: `sudo mkdir /opt/dotnet`
5. Extract the runtime to the directory: `sudo tar xzf dotnet-runtime-latest-linux-arm.tar.gz -C /opt/dotnet`
6. Create a softlink for the dotnet binary in /usr/local/bin: `sudo ln -s /opt/dotnet/dotnet /usr/local/bin/dotnet`
7. Clean up the installation: `rm dotnet-runtime-latest-linux-arm.tar.gz`
8. Verify that the installation was successful: `dotnet --info`
9. If you were successful, you should the Microsoft .NET Core Shared Framework Host version number.

Before deploying the final build, the project files will need to be modified to run on the installed runtime.

1. Add the following .NET Core MyGet feed to your NuGet sources: `https://dotnet.myget.org/F/dotnet-core/api/v3/index.json`
2. Open your .csproj file, and inside the root () element replace X.X.X with the runtime version installed on the Pi.

```
<PropertyGroup>
<RuntimeFrameworkVersion>X.X.X</RuntimeFrameworkVersion>
</PropertyGroup>
```

Input the following to build and publish the application:

1. Clean your previous build: `dotnet clean -c Release`
2. Restore packages: `dotnet restore`
3. Build your project in Release configuration for .NET Core 2.0: `dotnet build -c Release -f netcoreapp2.0`
4. Publish your project: `dotnet publish -c Release -f netcoreapp2.0`
5. The final build will be placed in bin/Release/netcoreapp2.0/publish. Package the folder contents, transfer them to your Pi, unpack and run using the command dotnet FlawBOT.dll

---

## DigitalOcean Deployment

1. Install a Ubuntu 18.04 DigitalOcean Droplet
2. Log into Ubuntu using DigitalOcean root and password
3. Change the password (automatically prompted)
4. Run the deployment scripts found below
5. Connect to FlawBOT via WinSCP and copy the config.json file

```
wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo dpkg --purge packages-microsoft-prod && sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get upgrade
sudo apt-get install dotnet-sdk-2.2
git clone https://github.com/CriticalFlaw/FlawBOT.git
cd FlawBOT/src/FlawBOT
dotnet restore
dotnet build --configuration Release
dotnet run --configuration Release
```