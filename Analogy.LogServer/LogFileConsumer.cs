using Analogy.LogServer.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Analogy.LogServer
{
    public class LogFileConsumer : ILogConsumer
    {
        private readonly ILogger logger;
        public LogFileConsumer(ILogger logger)
        {
            this.logger = logger;
        }

        public Task ConsumeLog(AnalogyGRPCLogMessage msg)
        {
            switch (msg.Level)
            {

                case AnalogyGRPCLogLevel.None:
                case AnalogyGRPCLogLevel.Trace:
                    logger.LogTrace(msg.Text);
                    break;
                case AnalogyGRPCLogLevel.Verbose:
                case AnalogyGRPCLogLevel.Unknown:
                case AnalogyGRPCLogLevel.Information:
                case AnalogyGRPCLogLevel.Analogy:
                    logger.LogInformation(msg.Text);
                    break;
                case AnalogyGRPCLogLevel.Debug:
                    logger.LogDebug(msg.Text);
                    break;
                case AnalogyGRPCLogLevel.Warning:
                    logger.LogWarning(msg.Text);
                    break;
                case AnalogyGRPCLogLevel.Error:
                    logger.LogError(msg.Text);
                    break;
                case AnalogyGRPCLogLevel.Critical:
                    logger.LogCritical(msg.Text);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return Task.CompletedTask;
        }
    }
}
