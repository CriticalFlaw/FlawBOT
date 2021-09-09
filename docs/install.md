Follow these steps to run a version of FlawBOT on your local machine.
1. [Download the latest version of **flawbot.zip**][release-link].
2. Extract downloaded file contents into a separate folder.
3. Modify the included **config.json** file with your [API tokens][tokens-link].
4. Run the below command in the Command Prompt.
   * In case of errors, install the [.NET Core 5.0 Runtime for **console apps**][runtime-link].
```
dotnet FlawBOT.dll
```

---

Follow these steps to compile and run your own version of FlawBOT from source.
1. Clone the repository.
2. Open `FlawBOT\src\FlawBOT.sln` in [Visual Studio 2019][vs-link].
3. Modify the included **config.json** file with your [API tokens][tokens-link].
4. Build and run the project.