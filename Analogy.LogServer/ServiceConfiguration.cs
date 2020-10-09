using System.Collections.Generic;
using Analogy.Interfaces;

namespace Analogy.LogServer
{
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
}
