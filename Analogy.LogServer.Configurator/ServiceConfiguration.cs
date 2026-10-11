#pragma warning disable MA0048 // File name must match type name
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable SA1649 // File name should match first type name
using Analogy.Interfaces;
using Analogy.Interfaces.DataTypes;
using System.Collections.Generic;

namespace Analogy.LogServer.Configurator
{
    public class ServerConfiguration
    {
        public ServiceConfiguration ServiceConfiguration { get; set; }
        public Serilog Serilog { get; set; }
    }
    public class ServiceConfiguration
    {
        public int HoursToKeepHistory { get; set; }
        public int CleanUpIntervalMinutes { get; set; }
        public bool LogAlsoToLogFile { get; set; }
        public int MemoryUsageInMB { get; set; }

        public WindowsEventLogsConfiguration WindowsEventLogsConfiguration { get; set; }

        public ServiceConfiguration()
        {
        }
    }

    public class WindowsEventLogsConfiguration
    {
        public bool EnableMonitoring { get; set; }
        public bool SaveToLogFile { get; set; }
        public AnalogyLogLevel MinimumLogLevel { get; set; }
        public List<string> LogsToMonitor { get; set; }
    }

    public class Serilog
    {
        public object[] Usings { get; set; }
        public Minimumlevel MinimumLevel { get; set; }
        public string[] Enrich { get; set; }
        public Writeto[] WriteTo { get; set; }
    }

    public class Minimumlevel
    {
        public string Default { get; set; }
        public Override Override { get; set; }
    }

    public class Override
    {
        public string Microsoft { get; set; }
        public string Grpc { get; set; }
        public string MicrosoftHostingLifetime { get; set; }
        public string System { get; set; }
    }

    public class Writeto
    {
        public string Name { get; set; }
        public Args Args { get; set; }
    }

    public class Args
    {
        public string PathFormat { get; set; }
        public string Formatter { get; set; }
    }
}