using Analogy.Interfaces;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace Analogy.LogServer.Clients.Test.Console
{
#if NETCOREAPP3_1_OR_GREATER

    public class Program
    {    
        public static TargetFrameworkAttribute CurrentFrameworkAttribute => (TargetFrameworkAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(TargetFrameworkAttribute));

        static async Task Main(string[] args)
        {
            string ip = "localhost";
            if (args.Length >= 2)
            {
                ip = args[1];
            }

            var p = new AnalogyMessageProducer($"http://{ip}:6000", null);
            var ai = new Dictionary<string, string> { { "some key", "some value" } };
            for (int i = 0; i < 100000; i++)
            {
                await p.Log(text: $@"test {i} ({CurrentFrameworkAttribute.FrameworkName})", source: "none", additionalInformation: ai, level: AnalogyLogLevel.Information).ConfigureAwait(false);
                await Task.Delay(500).ConfigureAwait(false);
            }
        }
    }
#else
    public class Program
    {
        public static TargetFrameworkAttribute CurrentFrameworkAttribute => (TargetFrameworkAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(TargetFrameworkAttribute));
        static async Task Main(string[] args)
        {
            string ip = "localhost";
            if (args.Length >= 2)
            {
                ip = args[1];
            }

            var p = new AnalogyMessageProducer($"{ip}:6000");
            var ai = new Dictionary<string, string> { { "some key", "some value" } };
            for (int i = 0; i < 100000; i++)
            {
                await p.Log(text: $@"test {i} ({CurrentFrameworkAttribute.FrameworkName})", source: "none", additionalInformation: ai, level: AnalogyLogLevel.Information).ConfigureAwait(false);
                await Task.Delay(500).ConfigureAwait(false);
            }
        }
    }
#endif
}
