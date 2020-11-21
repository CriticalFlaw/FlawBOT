using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT
{
    internal sealed class Program
    {
        private static List<FlawBot> Shards { get; } = new List<FlawBot>();
        private static CancellationTokenSource CancelTokenSource { get; } = new CancellationTokenSource();

        public static void Main(string[] args)
        {
            RunBotAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task RunBotAsync(string[] args)
        {
            try
            {
                // TODO: Remove?
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                // Set a command for canceling the bot process
                Console.CancelKeyPress += ConsoleOnCancelKeyPress;

                // Load the bot configuration
                var service = new BotServices();
                if (!service.LoadConfiguration()) return;

                // Generate a list of shards
                var botList = new List<Task>();
                for (var i = 0; i < 1; i++) // TODO: Replace 1 with ShardCount
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
                if (!(ex.InnerException is null))
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