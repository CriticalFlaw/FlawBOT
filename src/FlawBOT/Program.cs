﻿using FlawBOT.Common;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlawBOT
{
    internal sealed class Program
    {
        private static List<FlawBot> Shards { get; } = new();
        private static CancellationTokenSource CancelTokenSource { get; } = new();
        public static BotSettings Settings { get; set; }

        public static void Main(string[] args)
        {
            RunBotAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task RunBotAsync(string[] args)
        {
            try
            {
                // Set a command for canceling the bot process
                Console.CancelKeyPress += ConsoleOnCancelKeyPress;

                // Load Settings
                var fileName = "Resources\\config.json";
                if (!File.Exists(fileName)) return;
                var json = await new StreamReader(File.OpenRead(fileName), new UTF8Encoding(false)).ReadToEndAsync();
                Settings = JsonConvert.DeserializeObject<BotSettings>(json);

                // Generate a list of shards
                var botList = new List<Task>();
                for (var i = 0; i < Settings?.ShardCount; i++)
                {
                    var client = new FlawBot(i);
                    Shards.Add(client);
                    botList.Add(client.RunAsync());
                    await Task.Delay(7500).ConfigureAwait(false);
                }

                // Run bot shards
                await Task.WhenAll(botList).ConfigureAwait(false);
                await Task.Delay(Timeout.Infinite).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.ERR_EXCEPTION, ex.Message);
                if (ex.InnerException is not null)
                    Console.WriteLine(Resources.ERR_EXCEPTION_INNER, ex.InnerException.Message);
                Console.ReadKey();
            }

            Console.WriteLine(Resources.INFO_SHUTDOWN);
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;

            foreach (var shard in Shards)
                shard.StopAsync().GetAwaiter().GetResult();

            CancelTokenSource.Cancel();
        }
    }
}