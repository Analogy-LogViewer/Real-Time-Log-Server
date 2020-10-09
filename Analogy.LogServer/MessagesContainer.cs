using Analogy.LogServer.Interfaces;
using Analogy.LogServer.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analogy.LogServer
{
    public class MessagesContainer
    {
        private readonly BlockingCollection<AnalogyGRPCLogMessage> messages;

        private Task _consumer;
        private readonly List<ILogConsumer> _consumers;
        private ILogger<MessagesContainer> _logger;
        private readonly ReaderWriterLockSlim _sync = new ReaderWriterLockSlim();
        public MessagesContainer(ServiceConfiguration serviceConfiguration, GRPCLogConsumer grpcLogConsumer, MessageHistoryContainer historyContainer, ILogger<MessagesContainer> logger)
        {
            _logger = logger;
            _consumers = new List<ILogConsumer>();
            AddConsumer(grpcLogConsumer);
            if (serviceConfiguration.LogAlsoToLogFile)
            {
                AddConsumer(new LogFileConsumer(logger));
            }
            if (serviceConfiguration.HoursToKeepHistory > 0)
            {
                AddConsumer(historyContainer);
            }

            messages = new BlockingCollection<AnalogyGRPCLogMessage>();
            _consumer = Task.Factory.StartNew(async () =>
            {
                foreach (var msg in messages.GetConsumingEnumerable())
                {
                    try
                    {
                        _sync.EnterReadLock();
                        await Task.WhenAll(_consumers.Select(c => c.ConsumeLog(msg)).ToArray());
                    }
                    finally
                    {
                        _sync.ExitReadLock();
                    }
                }
            });
        }

        public void AddMessage(AnalogyGRPCLogMessage m) => messages.Add(m);

        public void AddConsumer(ILogConsumer consumer)
        {
            try
            {
                _sync.EnterWriteLock();
                if (!_consumers.Contains(consumer))
                {
                    _logger.LogInformation("Add new consumer: {consumer}", consumer);
                    _consumers.Add(consumer);
                }
            }
            finally
            {
                _sync.ExitWriteLock();
            }
        }

        public void RemoveConsumer(ILogConsumer consumer)
        {
            try
            {
                _sync.EnterWriteLock();
                _consumers.Remove(consumer);
            }
            finally
            {
                _sync.ExitWriteLock();
            }
        }


        public void Stop()
        {
            messages.CompleteAdding();
        }
    }
}
