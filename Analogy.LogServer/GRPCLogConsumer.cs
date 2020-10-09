using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Analogy.LogServer.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Analogy.LogServer
{
    public class GRPCLogConsumer : ILogConsumer
    {
        private readonly ILogger<GRPCLogConsumer> _logger;
        private List<(IServerStreamWriter<AnalogyGRPCLogMessage> stream, bool active)> clients;
        private readonly ReaderWriterLockSlim _sync = new ReaderWriterLockSlim();
        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        public GRPCLogConsumer(ILogger<GRPCLogConsumer> logger)
        {
            _logger = logger;
            clients = new List<(IServerStreamWriter<AnalogyGRPCLogMessage> stream, bool active)>();
        }

        public void AddGrpcConsumer(string requestMessage, IServerStreamWriter<AnalogyGRPCLogMessage> responseStream)
        {
            try
            {
                _logger.LogInformation("Adding client with message: {message}", requestMessage);
                _sync.EnterWriteLock();
                clients.Add((responseStream, true));
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
                _logger.LogInformation("Removing client with message: {message}");
                _sync.EnterWriteLock();
                var exist = clients.Exists(c => c.stream == responseStream);
                if (exist)
                    clients.RemoveAll(c => c.stream == responseStream);

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


                _sync.EnterReadLock();
                for (int i = 0; i < clients.Count; i++)
                {
                    var (stream, active) = clients[i];
                    if (!active) continue;
                    try
                    {
                        await _semaphoreSlim.WaitAsync();
                        await stream.WriteAsync(msg);
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
            finally
            {
                _sync.ExitReadLock();
            }
        }

        public override string ToString() => $"gRPC consumer";
    }
}
