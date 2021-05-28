// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

using CommandLine;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetListRefs
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Options options = null;

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o => options = o);

            if (options == null)
            {
                return 1;
            }

            var services = new ServiceCollection();

            Startup.ConfigureServices(services, options);

            using (var provider = services.BuildServiceProvider())
            using (var tokenSource = new CancellationTokenSource())
            {
                // Shut down semi-gracefully on ctrl+c...
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    Console.WriteLine("*** Cancel event triggered ***");
                    tokenSource.Cancel();
                    eventArgs.Cancel = true;
                };

                try
                {
                    var app = provider.GetRequiredService<App>();

                    await app.RunAsync(options, tokenSource.Token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unhandled exception!");
                    Console.WriteLine(ex);
                }
            }

            return 0;
        }
    }
}
