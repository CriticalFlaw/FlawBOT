using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace FlawBOT
{
    internal static class Program
    {
        public static BotSettings Settings { get; set; } = new();

        /// <summary>
        ///     Location of the configuration file, relative to the application rather than the working directory.
        /// </summary>
        public static string ConfigPath { get; } = Path.Combine(AppContext.BaseDirectory, "Resources", "config.json");

        public static async Task<int> Main(string[] args)
        {
            using var cancelTokenSource = new CancellationTokenSource();

            // Shut down cleanly on Ctrl+C (SIGINT) and on SIGTERM (systemd, Docker).
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cancelTokenSource.Cancel();
            };
            using var sigterm = PosixSignalRegistration.Create(PosixSignal.SIGTERM, ctx =>
            {
                ctx.Cancel = true;
                cancelTokenSource.Cancel();
            });

            var shards = new List<FlawBot>();
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    Console.Error.WriteLine($"Configuration file not found: {ConfigPath}");
                    return 1;
                }

                Settings = LoadSettings(await File.ReadAllTextAsync(ConfigPath, cancelTokenSource.Token).ConfigureAwait(false));
                if (string.IsNullOrWhiteSpace(Settings.Tokens.DiscordToken))
                {
                    Console.Error.WriteLine($"A Discord bot token is required. Add it to {ConfigPath} under Tokens > Discord.");
                    return 1;
                }

                // Download the Steam app list and TF2 item schema in the background so the bot can connect right away.
                _ = Task.Run(() => RefreshSteamDataAsync(Settings.Tokens.SteamToken));

                // Start each shard, waiting between them as Discord limits how quickly sessions can be identified.
                for (var i = 0; i < Settings.ShardCount; i++)
                {
                    var client = new FlawBot(i);
                    shards.Add(client);
                    // Connecting retries until Discord is reachable, so stop waiting if shutdown is requested.
                    await client.RunAsync().WaitAsync(cancelTokenSource.Token).ConfigureAwait(false);
                    if (i + 1 < Settings.ShardCount)
                        await Task.Delay(TimeSpan.FromSeconds(7.5), cancelTokenSource.Token).ConfigureAwait(false);
                }

                await Task.Delay(Timeout.Infinite, cancelTokenSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancelTokenSource.IsCancellationRequested)
            {
                // Shutdown requested.
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(Resources.ERR_EXCEPTION, ex.Message);
                if (ex.InnerException is not null)
                    Console.Error.WriteLine(Resources.ERR_EXCEPTION_INNER, ex.InnerException.Message);
                return 1;
            }
            finally
            {
                Console.WriteLine(Resources.INFO_SHUTDOWN);
                foreach (var shard in shards)
                {
                    try
                    {
                        await shard.StopAsync().WaitAsync(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                    }
                    catch (TimeoutException)
                    {
                        Console.Error.WriteLine("Timed out while disconnecting from Discord.");
                    }
                }
            }

            return 0;
        }

        /// <summary>
        ///     Parses the configuration file contents. Missing sections fall back to their defaults.
        /// </summary>
        public static BotSettings LoadSettings(string json)
        {
            var settings = JsonConvert.DeserializeObject<BotSettings>(json) ?? new BotSettings();
            settings.Lavalink ??= new Lavalink();
            settings.Tokens ??= new Tokens();
            settings.ShardCount = Math.Max(1, settings.ShardCount);
            if (string.IsNullOrWhiteSpace(settings.Name)) settings.Name = "FlawBOT";
            return settings;
        }

        private static async Task RefreshSteamDataAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("No Steam token provided; Steam and TF2 item lookups will be unavailable.");
                return;
            }

            var results = await Task.WhenAll(SteamService.UpdateSteamAppListAsync(token), TeamFortressService.UpdateTF2SchemaAsync(token)).ConfigureAwait(false);
            if (results.All(x => x)) Console.WriteLine("Steam app list and TF2 item schema loaded.");
        }
    }
}
