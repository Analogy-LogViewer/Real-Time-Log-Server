#if NETCOREAPP3_1 || NET
using Analogy.Interfaces;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Analogy.LogServer.Clients
{
    public class AnalogyMessageConsumer : IDisposable
    {
        private static Analogy.AnalogyClient Client { get; set; }
        private readonly AsyncServerStreamingCall<AnalogyGRPCLogMessage> _stream;
        private CancellationTokenSource _cts;
        private GrpcChannel channel;
        static AnalogyMessageConsumer()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        public AnalogyMessageConsumer(string address)
        {
            channel = GrpcChannel.ForAddress(address);
            Client = new Analogy.AnalogyClient(channel);
            AnalogyConsumerMessage m = new AnalogyConsumerMessage { Message = "client" };
            _stream = Client.SubscribeForConsumingMessages(m);
        }

        public async IAsyncEnumerable<Interfaces.AnalogyLogMessage> GetMessages()
        {
            _cts = new CancellationTokenSource();
            await foreach (var m in _stream.ResponseStream.ReadAllAsync())
            {
                var token = _cts.Token;
                Interfaces.AnalogyLogMessage msg = new Interfaces.AnalogyLogMessage()
                {
                    Level = (AnalogyLogLevel)m.Level,
                    Class = (AnalogyLogClass)m.Class,
                    Date = m.Date.ToDateTimeOffset(),
                    FileName = m.FileName,
                    LineNumber = m.LineNumber,
                    MachineName = m.MachineName,
                    MethodName = m.MethodName,
                    Module = m.Module,
                    ProcessId = m.ProcessId,
                    Source = m.Source,
                    Text = m.Text,
                    ThreadId = m.ThreadId,
                    User = m.User,
                };
                if (!string.IsNullOrEmpty(m.Category))
                {
                    msg.AddOrReplaceAdditionalProperty("Category", m.Category);
                }
                msg.Id = string.IsNullOrEmpty(m.Id)
                    ? Guid.NewGuid()
                    : Guid.TryParse(m.Id, out Guid id) ? id : Guid.NewGuid();
                yield return msg;
                if (token.IsCancellationRequested)
                {
                    yield break;
                }
            }
        }
        public Task Stop()
        {
            _cts?.Cancel();
            return channel.ShutdownAsync();
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _stream?.Dispose();
            channel?.Dispose();
        }
    }
}
#endif