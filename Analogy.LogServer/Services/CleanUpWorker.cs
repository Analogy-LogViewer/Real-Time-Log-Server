using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Analogy.LogServer.Services
{
    public class CleanUpWorker : BackgroundService
    {
        private ILogger<GreeterService> Logger { get; }
        private MessagesContainer MessageContainer { get; }
        private MessageHistoryContainer HistoryContainer { get; }
        private ServiceConfiguration ServiceConfiguration { get; }
        private Process CurrentProcess { get; } = Process.GetCurrentProcess();

        public CleanUpWorker(MessagesContainer messageContainer, MessageHistoryContainer historyContainer, ServiceConfiguration configuration, ILogger<GreeterService> logger)
        {
            Logger = logger;
            ServiceConfiguration = configuration;
            MessageContainer = messageContainer;
            HistoryContainer = historyContainer;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(ServiceConfiguration.CleanUpIntervalMinutes * 60 * 1000, stoppingToken).ConfigureAwait(false);
                    HistoryContainer.CleanMessages(ServiceConfiguration.HoursToKeepHistory);
                    if (CurrentProcess.PrivateMemorySize64 / 1024 / 1024 > ServiceConfiguration.MemoryUsageInMB)
                    {
                        HistoryContainer.CleanMessagesByHalf();
                    }

                }
                catch (TaskCanceledException)
                {
                    Logger.LogInformation("Cancellation requested");
                    return;
                }
            }
        }
    }
}
