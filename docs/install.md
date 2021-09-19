Follow these steps to run a version of FlawBOT on your local machine.

1. [Download the latest version of **flawbot.zip**][releases-link].
2. Extract downloaded file contents into a separate folder.
3. Modify the included **config.json** file with your [API tokens][tokens-link].
4. Open the command prompt and start the bot by entering: `dotnet FlawBOT.dll`
   * In case of errors, install the [.NET Core 5.0 Runtime for **console apps**][runtime-link].
 
---

Follow these steps to compile and run your own version of FlawBOT from source.

1. Clone the repository.
2. Open `FlawBOT\src\FlawBOT.sln` in [Visual Studio 2019][vs-link].
3. Modify the included **config.json** file with your [API tokens][tokens-link].
4. Build and run the project.

<!-- MARKDOWN LINKS -->
[releases-link]: https://github.com/CriticalFlaw/FlawBOT/releases/latest
[tokens-link]: https://www.flawbot.criticalflaw.ca/tokens/
[vs-link]: https://visualstudio.microsoft.com/downloads/
[runtime-link]: https://dotnet.microsoft.com/download/dotnet/5.0/runtime