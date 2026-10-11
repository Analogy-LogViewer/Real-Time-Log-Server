#pragma warning disable RCS1169 // Make field read-only
#pragma warning disable SA1316
using Analogy.LogServer.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analogy.LogServer
{
    public class GRPCLogConsumer : ILogConsumer
    {
        private readonly ILogger<GRPCLogConsumer> logger;

        private List<(IServerStreamWriter<AnalogyGRPCLogMessage> stream, bool active)> clients;
        private List<(IServerStreamWriter<AnalogyGRPCLogMessage> stream, bool add)> pendingClients;
        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();
        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

        public GRPCLogConsumer(ILogger<GRPCLogConsumer> logger)
        {
            this.logger = logger;
            clients = new List<(IServerStreamWriter<AnalogyGRPCLogMessage> stream, bool active)>();
            pendingClients = new List<(IServerStreamWriter<AnalogyGRPCLogMessage> stream, bool add)>();
        }

        public void AddGrpcConsumer(string requestMessage, IServerStreamWriter<AnalogyGRPCLogMessage> responseStream)
        {
            try
            {
                logger.LogInformation("Adding client with message: {message}", requestMessage);
                sync.EnterWriteLock();
                pendingClients.Add((responseStream, true));
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

        public void RemoveGrpcConsumer(IServerStreamWriter<AnalogyGRPCLogMessage> responseStream)
        {
            try
            {
                logger.LogInformation("removing client with message");
                sync.EnterWriteLock();
                pendingClients.Add((responseStream, false));
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

        public async Task ConsumeLog(AnalogyGRPCLogMessage msg)
        {
            try
            {
                await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                if (pendingClients.Any())
                {
                    foreach ((IServerStreamWriter<AnalogyGRPCLogMessage> stream, bool add) pendingClient in
                        pendingClients)
                    {
                        if (pendingClient.add)
                        {
                            clients.Add((pendingClient.stream, true));
                        }
                        else
                        {
                            clients.RemoveAll(c => c.stream == pendingClient.stream);
                        }
                    }
                }
                pendingClients.Clear();
            }
            finally
            {
                semaphoreSlim.Release();
            }

            for (int i = 0; i < clients.Count; i++)
            {
                var (stream, active) = clients[i];
                if (!active)
                {
                    continue;
                }

                try
                {
                    await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                    await stream.WriteAsync(msg).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    clients[i] = (stream, false);
                    logger.LogDebug(e, "Error sending message");
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }
        }

        public override string ToString() => "gRPC consumer";
    }
}