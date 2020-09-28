namespace Analogy.LogServer
{
    public class ServiceConfiguration
    {
        public int HoursToKeepHistory { get; set; }

        public int CleanUpIntervalMinutes { get; set; }
        public bool LogAlsoToLogFile { get; set; }
        public int MemoryUsageInMB { get; set; }
        public ServiceConfiguration()
        {
        }
    }

}
