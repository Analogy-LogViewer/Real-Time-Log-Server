using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analogy.Interfaces;
using Analogy.LogServer.Interfaces;
using Microsoft.Extensions.Logging;

namespace Analogy.LogServer
{
    public class LogFileConsumer : ILogConsumer
    {
        private readonly ILogger logger;
        public LogFileConsumer(ILogger logger)
        {
            this.logger = logger;
        }

        public Task ConsumeLog(AnalogyLogMessage msg)
        {
            switch (msg.Level)
            {

                case "Disabled":
                case "None":
                    break;
                case "Trace":
                    logger.LogTrace(msg.Text);
                    break;
                case "Verbose":
                case "Unknown":
                case "Event":
                case "Information":
                case "AnalogyInformation":
                case "Analogy":
                    logger.LogInformation(msg.Text);
                    break;
                case "Debug":
                    logger.LogDebug(msg.Text);
                    break;
                case "Warning":
                    logger.LogWarning(msg.Text);
                    break;
                case "Error":
                    logger.LogError(msg.Text);
                    break;
                case "Fatal":
                case "Critical":
                    logger.LogCritical(msg.Text);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return Task.CompletedTask;
        }
    }
}
