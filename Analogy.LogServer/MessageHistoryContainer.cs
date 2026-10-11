using Analogy.LogServer.Interfaces;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analogy.LogServer
{
    public class MessageHistoryContainer : ILogConsumer
    {
        private SemaphoreSlim semaphoreSlim;
        private List<AnalogyGRPCLogMessage> OldMessages;

        public MessageHistoryContainer()
        {
            OldMessages = new List<AnalogyGRPCLogMessage>();
            semaphoreSlim = new SemaphoreSlim(1);
        }

        public async Task ConsumeLog(AnalogyGRPCLogMessage msg)
        {
            try
            {
                await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                OldMessages.Add(msg);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task CleanMessages(int deleteOlderHours)
        {
            if (deleteOlderHours < 1)
            {
                return;
            }
            try
            {
                await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                OldMessages.RemoveAll(m => m.Date <= Timestamp.FromDateTime(DateTime.Now.AddHours(-deleteOlderHours).ToUniversalTime()));
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task CleanMessagesByHalf()
        {
            try
            {
                await semaphoreSlim.WaitAsync().ConfigureAwait(false); if (OldMessages.Any())
                {
                    OldMessages.RemoveRange(0, OldMessages.Count / 2);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task<List<AnalogyGRPCLogMessage>> GetOldMessages()
        {
            try
            {
                await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                return OldMessages.ToList();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}