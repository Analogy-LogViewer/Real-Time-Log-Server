using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Analogy.LogViewer.gRPCLogServer
{
    public class AnalogyViewerSender
    {
        private readonly BlockingCollection<AnalogyLogMessage> messages;
        private List<(IServerStreamWriter<AnalogyLogMessage> stream, bool active)> clients;
        private ReaderWriterLockSlim sync = new ReaderWriterLockSlim();
        private Task producer;
        private ILogger<AnalogyViewerSender> _logger;

        public AnalogyViewerSender(ILogger<AnalogyViewerSender> logger)
        {
            _logger = logger;
            messages = new BlockingCollection<AnalogyLogMessage>();
            clients = new List<(IServerStreamWriter<AnalogyLogMessage> stream, bool active)>();
            producer = Task.Factory.StartNew(async () =>
            {
                foreach (var msg in messages.GetConsumingEnumerable())
                {
                    for (int i = 0; i < clients.Count; i++)
                    {
                        var (stream, active) = clients[i];
                        if (!active) continue;
                        try
                        {
                            await stream.WriteAsync(msg);
                        }
                        catch (Exception e)
                        {
                            clients[i] = (stream, false);
                            logger.LogDebug(e, "Error sending message");
                        }
                    }
                }
            });
        }
        public void AddMessage(AnalogyLogMessage m) => messages.Add(m);

        public void AddConsumer(string requestMessage, IServerStreamWriter<AnalogyLogMessage> responseStream)
        {
            try
            {
                sync.EnterWriteLock();
                clients.Add((responseStream,true));
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }
    }
}
