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
        private SemaphoreSlim _semaphoreSlim;
        private List<AnalogyGRPCLogMessage> _OldMessages;

        public MessageHistoryContainer()
        {
            _OldMessages = new List<AnalogyGRPCLogMessage>();
            _semaphoreSlim = new SemaphoreSlim(1);
        }

        public async Task ConsumeLog(AnalogyGRPCLogMessage msg)
        {
            try
            {
                await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
                _OldMessages.Add(msg);
            }
            finally
            {
                _semaphoreSlim.Release();
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
                await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
                _OldMessages.RemoveAll(m => m.Date <= Timestamp.FromDateTime(DateTime.Now.AddHours(-deleteOlderHours).ToUniversalTime()));
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task CleanMessagesByHalf()
        {
            try
            {
                await _semaphoreSlim.WaitAsync().ConfigureAwait(false); if (_OldMessages.Any())
                {
                    _OldMessages.RemoveRange(0, _OldMessages.Count / 2);
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<List<AnalogyGRPCLogMessage>> GetOldMessages()
        {
            try
            {
                await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
                return _OldMessages.ToList();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}