using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analogy.Interfaces;

namespace Analogy.LogServer.Clients.Test.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string ip = "localhost";
            if (args.Length >= 2)
                ip = args[1];
            var p = new AnalogyMessageProducer($"http://{ip}:6000", null);
            var ai = new Dictionary<string, string> { { "some key", "some value" } };
            for (int i = 0; i < 100000; i++)
            {
                await p.Log(text: "test " + i, source: "none", additionalInformation: ai, level: AnalogyLogLevel.Information).ConfigureAwait(false);
                await Task.Delay(500).ConfigureAwait(false);
            }
        }
    }
}
