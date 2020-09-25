namespace Analogy.LogServer
{
    public class CommonSystemConfiguration
    {
        public int DaysToKeeps { get; set; }

        public int CleanUpIntervalMinutes { get; set; }
        public bool LogAlsoToLogFile { get; set; }
        public CommonSystemConfiguration()
        {
        }
    }

}
