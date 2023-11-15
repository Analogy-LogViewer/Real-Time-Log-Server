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
        private readonly ILogger<GRPCLogConsumer> _logger;
        private List<(IServerStreamWriter<AnalogyGRPCLogMessage> stream, bool active)> clients;
        private List<(IServerStreamWriter<AnalogyGRPCLogMessage> stream, bool add)> pendingClients;
        private readonly ReaderWriterLockSlim _sync = new ReaderWriterLockSlim();
        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private List<(IServerStreamWriter<AnalogyGRPCLogMessage> stream, bool active)> ActiveClients { get; set; }

        public GRPCLogConsumer(ILogger<GRPCLogConsumer> logger)
        {
            _logger = logger;
            clients = new List<(IServerStreamWriter<AnalogyGRPCLogMessage> stream, bool active)>();
            pendingClients = new List<(IServerStreamWriter<AnalogyGRPCLogMessage> stream, bool add)>();
        }

        public void AddGrpcConsumer(string requestMessage, IServerStreamWriter<AnalogyGRPCLogMessage> responseStream)
        {
            try
            {
                _logger.LogInformation("Adding client with message: {message}", requestMessage);
                _sync.EnterWriteLock();
                pendingClients.Add((responseStream, true));
            }
            finally
            {
                _sync.ExitWriteLock();
            }
        }

        public void RemoveGrpcConsumer(IServerStreamWriter<AnalogyGRPCLogMessage> responseStream)
        {
            try
            {
                _logger.LogInformation("removing client with message");
                _sync.EnterWriteLock();
                pendingClients.Add((responseStream, false));
            }
            finally
            {
                _sync.ExitWriteLock();
            }
        }

        public async Task ConsumeLog(AnalogyGRPCLogMessage msg)
        {
            try
            {
                await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
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
                _semaphoreSlim.Release();
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
                    await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
                    await stream.WriteAsync(msg).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    clients[i] = (stream, false);
                    _logger.LogDebug(e, "Error sending message");
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }
        }

        public override string ToString() => $"gRPC consumer";
    }
}