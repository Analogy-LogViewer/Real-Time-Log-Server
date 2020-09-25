using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Analogy.LogServer.Services
{
    public class CleanUpWorker : BackgroundService
    {
        private ILogger<GreeterService> Logger { get; }
        private MessagesContainer MessageContainer { get; }
        private CommonSystemConfiguration SystemConfiguration { get; }

        public CleanUpWorker(MessagesContainer messageContainer, CommonSystemConfiguration configuration, ILogger<GreeterService> logger)
        {
            Logger = logger;
            SystemConfiguration = configuration;
            MessageContainer = messageContainer;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(SystemConfiguration.CleanUpIntervalMinutes*60*1000, stoppingToken).ConfigureAwait(true);
                }
                catch (TaskCanceledException)
                {
                    Logger.LogInformation("Cancellation requested");
                    return;
                }

                try
                {
                  
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Error cleanup.");
                }


            }
        }
    }
}
