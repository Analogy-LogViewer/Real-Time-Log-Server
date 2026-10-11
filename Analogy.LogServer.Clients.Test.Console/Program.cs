using Analogy.Interfaces;
using Analogy.Interfaces.DataTypes;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace Analogy.LogServer.Clients.Test.Console
{
#if NET
    public static class Program
    {
        public static TargetFrameworkAttribute CurrentFrameworkAttribute => Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>();

        public static async Task Main(string[] args)
        {
            string ip = "localhost";
            if (args.Length >= 2)
            {
                ip = args[1];
            }

            var p = new AnalogyMessageProducer($"http://{ip}:6000");
            var ai = new Dictionary<string, string>(System.StringComparer.Ordinal) { { "some key", "some value" } };
            for (int i = 0; i < 100000; i++)
            {
                await p.Log(text: string.Create(CultureInfo.InvariantCulture, $"test {i} ({CurrentFrameworkAttribute.FrameworkName})"), source: "none", level: AnalogyLogLevel.Information, additionalInformation: ai).ConfigureAwait(false);
                await Task.Delay(500).ConfigureAwait(false);
            }
        }
    }
#else
    public static class Program
    {
        public static TargetFrameworkAttribute CurrentFrameworkAttribute => (TargetFrameworkAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(TargetFrameworkAttribute));
        public static async Task Main(string[] args)
        {
            string ip = "localhost";
            if (args.Length >= 2)
            {
                ip = args[1];
            }

            var p = new AnalogyMessageProducer($"{ip}:6000");
            var ai = new Dictionary<string, string>(System.StringComparer.Ordinal) { { "some key", "some value" } };
            for (int i = 0; i < 100000; i++)
            {
#pragma warning disable MA0076 // Do not use implicit culture-sensitive ToString in interpolated strings
                await p.Log(text: $"test {i} ({CurrentFrameworkAttribute.FrameworkName})", source: "none", level: AnalogyLogLevel.Information, additionalInformation: ai).ConfigureAwait(false);
#pragma warning restore MA0076 // Do not use implicit culture-sensitive ToString in interpolated strings
                await Task.Delay(500).ConfigureAwait(false);
            }
        }
    }
#endif
}