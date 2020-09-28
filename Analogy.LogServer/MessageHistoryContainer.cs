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
        private readonly ReaderWriterLockSlim _sync;

        private List<AnalogyLogMessage> _OldMessages;

        public MessageHistoryContainer()
        {
            _OldMessages = new List<AnalogyLogMessage>();
            _sync = new ReaderWriterLockSlim();
        }

        public Task ConsumeLog(AnalogyLogMessage msg)
        {
            try
            {
                _sync.EnterWriteLock();
                _OldMessages.Add(msg);
            }
            finally
            {
                _sync.ExitWriteLock();
            }
            return Task.CompletedTask;
        }

        public void CleanMessages(int deleteOlderHours)
        {
            if (deleteOlderHours < 1)
            {
                return;
            }
            try
            {
                _sync.EnterWriteLock();
                _OldMessages.RemoveAll(m => m.Date <= Timestamp.FromDateTime(DateTime.Now.AddHours(-deleteOlderHours)));
            }
            finally
            {
                _sync.ExitWriteLock();
            }

        }

        public void CleanMessagesByHalf()
        {
            try
            {
                _sync.EnterWriteLock();
                if (_OldMessages.Any())
                    _OldMessages.RemoveRange(0, _OldMessages.Count / 2);
            }
            finally
            {
                _sync.ExitWriteLock();
            }
        }

        public List<AnalogyLogMessage> GetOldMessages()
        {
            try
            {
                _sync.EnterWriteLock();
                return _OldMessages.ToList();
            }
            finally
            {
                _sync.ExitWriteLock();
            }
        }
    }
}
